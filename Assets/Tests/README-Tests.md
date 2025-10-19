# Tests en VectorArcade (Unity 6)

Este documento explica cómo están organizados los tests del proyecto, cómo ejecutarlos (Editor, PlayMode y Player) y cómo resolver incidencias típicas (asmdef, escenas, paquete de tests, etc.). También recoge nuestro estándar de documentación de pruebas y los “fitness tests” de arquitectura.

---

## Estructura

```
Assets/
  Tests/
    README-Tests.md          ← este documento
    Common/
      TestFakes.cs           ← fakes reutilizables (ITimeProvider, IInputProvider)
    EditMode/
      VectorArcade.EditModeTests.asmdef
      PlayerControlUseCase_Tests.cs
      TickUseCase_Should_UpdateCooldowns.cs
    PlayMode/
      VectorArcade.PlayModeTests.asmdef
      GcAllocs_Per_Frame_Should_Be_Zero.cs
```

**Assemblies de producción** relevantes (deben existir y tener estos nombres):

- `VectorArcade.Domain` (`Assets/Domain/Domain.asmdef`)
- `VectorArcade.Application.Ports` (`Assets/Application/Ports/Application.Ports.asmdef`)
- `VectorArcade.Application` (`Assets/Application/Application.asmdef`)
- `VectorArcade.Infrastructure` (`Assets/Infrastructure/Infrastructure.asmdef`)
- (Opcional) `VectorArcade.Presentation`

**Dependencias de los tests:**

- `VectorArcade.EditModeTests.asmdef` → referencias: `VectorArcade.Domain`, `VectorArcade.Application`, `VectorArcade.Application.Ports`.  
  `includePlatforms: ["Editor"]`, `optionalUnityReferences: ["TestAssemblies"]`.
- `VectorArcade.PlayModeTests.asmdef` → referencias: `VectorArcade.Domain`, `VectorArcade.Application`, `VectorArcade.Application.Ports`, `VectorArcade.Infrastructure`.  
  `optionalUnityReferences: ["TestAssemblies"]`.

> Nota: Los nombres en `references` deben coincidir **exactamente** con la propiedad `"name"` de cada `.asmdef`.

---

## Tipos de test y cuándo usarlos

| Tipo         | Dónde vive              | Para qué sirve                                            | Requisitos                                    |
| ------------ | ----------------------- | --------------------------------------------------------- | --------------------------------------------- |
| **EditMode** | `Assets/Tests/EditMode` | Lógica pura (dominio y casos de uso sin UnityEngine)      | `Test Framework`, asmdef con **Editor**       |
| **PlayMode** | `Assets/Tests/PlayMode` | Integración y runtime (MonoBehaviours, escena en editor)  | `Test Framework`, asmdef de PlayMode          |
| **Player**   | (Test Runner → Player)  | Ejecutar PlayMode tests en un **player** (build temporal) | Lista de escenas válida en **Build Profiles** |

---

## Cómo ejecutar

1. **Abrir el Test Runner**  
   `Window → General → Test Runner` (o `Window → Test Framework`).

2. **EditMode**

   - Pestaña **EditMode** → `Run All`.

3. **PlayMode**

   - Pestaña **PlayMode** → `Run All`.

4. **Player** (opcional)
   - Pestaña **Player** → `Run All`.
   - Requiere una **Scene List** válida (ver sección _Build Profiles_).

---

## Build Profiles (Unity 6) — _Scenes In Build_

Para que el modo **Player** funcione, la lista de escenas debe ser correcta:

1. `File → Build Profiles… → Scene List`.
2. Elimina escenas inexistentes (por ejemplo, la plantilla `Scenes/SampleScene.unity`).
3. Añade tu escena real, p. ej. `Presentation/Scenes/Main` (con **Add Open Scenes**).
4. Vuelve al Test Runner (pestaña **Player**) y ejecuta.

---

## Fitness tests (arquitectura)

Hemos adoptado _fitness functions_ para proteger las _-ilities_ del proyecto:

- **Performance (GC)**  
  `Assets/Tests/PlayMode/GcAllocs_Per_Frame_Should_Be_Zero.cs`  
  Mide el **delta** de “GC Allocated In Frame” entre un **baseline sin renderer** y una medición con `LineMeshBatchRenderer`.  
  El delta debe ser ≤ **1 KiB**. Así aseguramos que el renderer **no añade basura por frame**.

- **Determinismo / Correctitud de UC**  
  `Assets/Tests/EditMode/PlayerControlUseCase_Tests.cs`  
  Verifica avance estable en +Z y el **clamp de pitch**.  
  `Assets/Tests/EditMode/TickUseCase_Should_UpdateCooldowns.cs`  
  Verifica **cooldowns**, **vida de balas** y **compactación** (entidades muertas se eliminan).

> Todos los tests incluyen **comentarios XML** explicando escenario, pasos y resultados esperados.

---

## Estándar de documentación para tests

En cada archivo de test:

- Encabezado con `/// <summary>` describiendo **qué valida** y **por qué**.
- En cada `[Test]`, `/// <summary>` con **escenario**, **datos**, **pasos** y **criterios de éxito**.
- Opcional: `[Category("Fitness/Performance")]`, `[Category("Gameplay")]`, etc., para filtrar en Test Runner.

Ejemplo de etiqueta:

```csharp
[Category("Fitness/Performance")]
public class GcAllocs_Per_Frame_Should_Be_Zero { ... }
```

---

## Solución de problemas

### 1) “`UnityTestAttribute` not found”

- El asmdef correspondiente **no** está marcado como **Test Assemblies**.  
  Abre el `.asmdef` del test y asegúrate de tener `optionalUnityReferences: ["TestAssemblies"]`.

### 2) “`The type or namespace name 'VectorArcade' could not be found`”

- Falta **reference** en el `.asmdef` de tests hacia `VectorArcade.Domain` / `VectorArcade.Application` / `VectorArcade.Application.Ports` / `VectorArcade.Infrastructure`.
- El archivo con _fakes_ debe estar **dentro** del assembly del test (p. ej. `Assets/Tests/EditMode/Common/TestFakes.cs`).

### 3) “`is in an incorrect path for a scene file`”

- En **Build Profiles → Scene List** hay escenas inexistentes (p. ej. `Scenes/SampleScene`).
- Elimínalas con **clic derecho → Remove Scene** y deja solo `Presentation/Scenes/Main`.

### 4) “Sigue marcando en rojo en el IDE”

- En Unity: `Edit → Preferences → External Tools → Regenerate project files`.
- `Assets → Reimport All`.
- Vuelve a abrir el proyecto desde **Open C# Project**.

---

## Consejos de rendimiento en tests

- Evita `Where(...).ToList()` dentro del ciclo de juego. Usa `RemoveAll` o compaction manual **in-place**.
- En `LineMeshBatchRenderer`:
  - `_mesh.MarkDynamic();`
  - `_mesh.Clear(false);`
  - `_mesh.bounds = new Bounds(Vector3.zero, new Vector3(10000,10000,10000));`
  - Pre-reserva capacidad de listas (`_verts.Capacity`, `_indices.Capacity`).
- No reasignes materiales por frame; usa `sharedMaterial` una vez en `Awake`.

---

## (Opcional) Ejecución por línea de comandos / CI

Unity permite lanzar tests desde CLI. Ejemplos genéricos:

```bash
# EditMode
"C:\Program Files\Unity\Editor\Unity.exe" -projectPath "PATH\VectorArcade" ^
  -runTests -testPlatform editmode ^
  -logFile logs\editmode.log ^
  -testResults logs\editmode-results.xml

# PlayMode (Editor)
"C:\Program Files\Unity\Editor\Unity.exe" -projectPath "PATH\VectorArcade" ^
  -runTests -testPlatform playmode ^
  -logFile logs\playmode.log ^
  -testResults logs\playmode-results.xml
```

> Para modo **Player**, asegúrate de que **Build Profiles** tenga la lista de escenas correcta antes de lanzar la CLI.

---

## Contacto / mantenimiento

- Mantener este README actualizado cuando:
  - Cambien nombres de assemblies `.asmdef`
  - Se añadan nuevas escenas o cambie su ruta
  - Se introduzcan nuevos _fitness tests_ o categorías
- Si un test de fitness falla, **no eliminarlo**: ajustar el código o revisar la tolerancia documentando la razón.

---
