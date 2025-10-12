# 🎮 VectorArcade

**VectorArcade** es un videojuego retro-futurista estilo _Atari vectorial_, inspirado en clásicos como **Lunar Lander**, **Asteroids** y **Battlezone**.  
Desarrollado en **Unity 6000.0.58f1 (URP, PC)** bajo una **arquitectura limpia (Clean Architecture)**, completamente modular y desacoplada de Unity en su dominio lógico.

---

## 🧱 Arquitectura

![Architecture Diagram](https://raw.githubusercontent.com/JordiGL/VectorArcade/refs/heads/main/Docs/architecture.puml?token=GHSAT0AAAAAADM7OGZFYTQ4ARVE5XKCZOXG2HLVBRA)

El proyecto sigue una estructura **Clean Architecture**, separando responsabilidades en cuatro capas:

```
VectorArcade/
│
├── Domain/              ← Reglas puras del juego (sin Unity)
│   ├── Core/            ← Tipos matemáticos (Vec3, etc.)
│   ├── Entities/        ← Entidades: Player, Asteroid, Bullet
│   └── Services/        ← Lógica pura (Physics, SpawnerRules)
│
├── Application/         ← Casos de uso (control del ciclo de juego)
│   ├── Ports/           ← Interfaces para infraestructura
│   └── UseCases/        ← TickUseCase, SpawnUseCase, ShootUseCase, etc.
│
├── Infrastructure/      ← Adaptadores técnicos (Unity)
│   ├── Rendering/       ← VectorLineRenderer (dibuja con MeshTopology.Lines)
│   ├── Input/           ← KeyboardMouseInputAdapter
│   ├── Time/            ← UnityTimeAdapter
│   └── Random/          ← UnityRandomAdapter
│
└── Presentation/        ← Capa visible (escena, bootstrap, HUD)
    ├── Bootstrap/       ← GameInstaller, GameController
    ├── HUD/             ← VectorHudPresenter (Score/FPS)
    ├── Debug/           ← SpawnerRulesLiveTuner (ajustes en tiempo real)
    └── Scenes/          ← Main.unity
```

---

## 🧭 Flujo general de ejecución

El juego sigue un **bucle principal** controlado desde `GameController`:

```
[Unity Update]
    ↓
[GameController]
    ├── TickUseCase → avanza la simulación (física, input, disparos)
    ├── SpawnUseCase → genera nuevos asteroides según reglas
    ├── AsteroidFieldUseCase → gestiona despawn y repoblación dinámica
    ↓
[GameState]
    ├── Player
    ├── Asteroids[]
    └── Bullets[]
    ↓
[WireframePresenter]
    → Envía líneas al ILineRendererPort (VectorLineRenderer)
    ↓
[VectorLineRenderer]
    → Renderiza con MeshTopology.Lines en un único batch
```

---

## 🧩 _Mapa de clases y relaciones_

```
Domain/
 ├─ Core/
 │   └─ Vec3                → vector matemático 3D puro
 │
 ├─ Entities/
 │   ├─ Player              → posición, dirección, vida, velocidad
 │   ├─ Asteroid            → posición, radio, estado
 │   └─ Bullet              → posición, dirección, vida útil
 │
 └─ Services/
     ├─ PhysicsService      → colisiones, movimiento
     ├─ SpawnerRules        → define parámetros de spawn/despawn
     └─ FieldRules          → densidad y radio de entorno

Application/
 ├─ Ports/
 │   ├─ IInputPort          → interfaz de entrada (teclado, mando…)
 │   ├─ ITimeProvider       → tiempo deltaTime abstracto
 │   ├─ IRandomProvider     → generador de aleatoriedad
 │   └─ ILineRendererPort   → API de dibujo vectorial
 │
 └─ UseCases/
     ├─ TickUseCase         → ciclo lógico: avanza, dispara, destruye
     ├─ SpawnUseCase        → genera asteroides en base a SpawnerRules
     └─ AsteroidFieldUseCase→ gestiona campo dinámico

Infrastructure/
 ├─ Rendering/
 │   └─ VectorLineRenderer  → implementa ILineRendererPort usando Mesh
 ├─ Input/
 │   └─ KeyboardMouseInputAdapter → implementa IInputPort
 ├─ Time/
 │   └─ UnityTimeAdapter    → implementa ITimeProvider
 └─ Random/
     └─ UnityRandomAdapter  → implementa IRandomProvider

Presentation/
 ├─ Bootstrap/
 │   ├─ GameInstaller       → inyecta dependencias y crea GameState
 │   └─ GameController      → loop principal Unity
 │
 ├─ HUD/
 │   ├─ VectorHudPresenter  → dibuja SCORE y FPS con líneas (estilo Atari)
 │   └─ WireframePresenter  → dibuja Player, Bullets, Asteroids
 │
 └─ Debug/
     └─ SpawnerRulesLiveTuner → permite modificar valores en tiempo real
```

---

## 🧠 Interacción entre capas

| Desde              | Hacia                        | Descripción                                                           |
| :----------------- | :--------------------------- | :-------------------------------------------------------------------- |
| **Presentation**   | **Application**              | Llama a los casos de uso (`TickUseCase`, `SpawnUseCase`)              |
| **Application**    | **Domain**                   | Manipula las entidades y servicios puros                              |
| **Application**    | **Infrastructure (puertos)** | Usa adaptadores concretos de Input, Time y Renderer                   |
| **Infrastructure** | **Unity Engine**             | Implementa los puertos mediante APIs de Unity (sin lógica de dominio) |

> El **Dominio nunca depende de Unity**: las físicas, las posiciones y el estado del juego son totalmente testeables fuera del motor.

---

## 🖥️ HUD vectorial (sin Canvas)

El HUD (puntuación y FPS) se dibuja en el espacio de cámara mediante el `VectorHudPresenter`, reutilizando el mismo sistema de líneas:

- **SCORE** → arriba-izquierda
- **FPS** → arriba-derecha
- (Próximamente: AST para asteroides visibles)

---

## ⚙️ Configuración de escena

En la escena `Main.unity`:

```
Main Camera (FOV 60, fondo negro)
└── GameRoot
     ├── GameInstaller
     ├── GameController
     ├── UnityTimeAdapter
     ├── KeyboardMouseInputAdapter
     ├── VectorLineRenderer
     └── SpawnerRulesLiveTuner
```

---

## 🧩 Dependencias

- Unity 6000.0.58f1
- URP (Universal Render Pipeline)
- Sin dependencias externas ni paquetes de terceros.

---

## 💾 Comandos Git útiles

```bash
git add .
git commit -m "feat: nueva lógica de spawn dinámico"
git push
```

---

## ✨ Próximos pasos

- Añadir AST count HUD (asteroides visibles)
- Añadir sonido retro (sintetizado proceduralmente)
- Implementar menú de opciones vectorial
- Futuro port: VR (Meta Quest 3, MR Core)

---

## 📜 Licencia

© 2025 Jordi Gómez. Todos los derechos reservados.
