# 🎮 VectorArcade

**VectorArcade** es un videojuego retro-futurista estilo _Atari vectorial_, inspirado en clásicos como **Lunar Lander**, **Asteroids** y **Battlezone**.  
Desarrollado en **Unity 6000.0.58f1 (URP, PC)** bajo una **arquitectura limpia (Clean Architecture)**, completamente modular y desacoplada de Unity en su dominio lógico.

---

## 🧱 Arquitectura

![Class Diagram](https://kroki.io/plantuml/svg/eNqlV9tuGzcQfd-vIPJSGZXdAn0TCiOyZDVC7VSQ4qR9KqjdkUSYIhck17KcGuhH9Av7JR1yL-KSVIKgT_ZyzsyQh4eH1FttqDLVnpOxynfMQG4qBX9OONV6yuhW0X1mmOFAPmJIKkTRAsi_f_9DJhyo6KWRgcsjTeJFlulHJkqK_xO9o4U8MLElG8o1eJG9FDLfKbkHYlTlR0qaP9ItrMwR-yvsQcWW-4Dcthsbo9i6MjDPpVixFyA_epACNrTiZiaFeU-xxUQKLTEty5rq5M1U7ikTb8jnjJBucCIV1EOkbmMJ-Ak_XzMfdiuQHQa6D11wegTVDBHy_UJqRElBRm0VNzyT6kBVQYLh1U5KM5GSI2E2ZcMlNS746vUYawNKsuKrXZa0YJUmpFcJx8ecPQFx42tsFjW4qTgH89XyH4HLnJljMHzHNq74t3b9BTdpZagBr3HN5qihtavUEKAxcse0-bkduG4R9QrqpTtEPdDFVzlucjNJJropfmA4BSZyOw9log3oC2AF6onloQBWJT0IUMuKgz4tZMaAF1PQTEExkZUw_b4u3O1WgjtMtXU9SAC4Z8J1nuJiKS4gAswFbiHlM8b5PZ4KVnLmmO2BljgzpCAAJXo1EwkD9DkRcKy19HwCWkoRsFNvzqoEKMKSdaiWVBCaIZlLlMsC1ArQAKLc--rlhcNvm40Gc35GE8k501bdnt5xb-ZI6A4UDMqRVfeQ5M1fNXKVLkIdv2avnrWMSyQvp_bYBP6ykMp0osFGoDYUN2w-F2VlFko-scJ3kHtZaZgCN_T3aIFd6I8UOQsFWjtKg_PmNbWKj3u6mjaU8CAveUlFIfdx-nt4NjObNtgzUbM1JHv6fCLuC0XvmIAlCCwIylLlyQS2TMzQ2mFw0VlBUdiEAX0e0uOQvgzXz8P1cbh-6RC3ovBz4nP8oGFCddrI8c4wSvIG0k3l8kgPAeOXJTP5LtyG22fI8X4aaOtro5PFXUTu94Hlj2Gbb0h3N8f_yG8d1DlRVKcxD7xhXfxMPQQ-lAV-fgkUn5S52Ciq8QHgXhLBYXGHor8zv8JxLfHydOp38XFBS-Ouh1c_1wq4n_qA6zja4TMZtaADQz8iNfs6ci7NyRVfOP1MK8x70LsbispoJR0t355SECblFDe4p0gMLft1LaNzgfSiNXqnbtvdnx7pbdBYeU0w4umsjWmnHRv0VdRGy_ogGMUdJHUuWujGiceiUnLq2vk35Kh3YbaQg3dLIMS7NJKvhmY2PTZqJfYOvcfsu4dpn9NP6JcbaxPNdjSPje79hw_gd1XhB90ufkeWwN3W6R0rNRmwfclhjxh9kZ0VKrm6-us6MPws1GYD8g06S2ixgfWtOEtKr4GGBmtX0XogPppLGxI5vmyJkcRdVj_U1Cct8fIyWsl5XG8xvuPF0Z6hJZpEcV8kSTdzRQKezgJ7qvQJOkj1SPCxUP90IM7kzq_4dA7D1Z4i0UpOobPTO0Fwbp0qNcFnkK7w2j69pXG9-AOq8Z5Y5UG1M4BYM_GRCCqdASTV51sgOeAUNIEnUEezQ0_N-nbX7xLHfH-Loz2Di8NJc4thSXdLNPM1FId9wQYu1i6ySwgBV1fXKctKoBLelUAltuUtflV7nv0HCkonRg==)

![Sequence Diagram](https://kroki.io/plantuml/svg/eNp1k0Fu2zAQRfc6xSxtNIX3XQRR3ChZ2IEQWfGmQDElJzYRilSHVFzveoheIxfo1jfpSTq0rNQC0p018_7wcz59FSJy7BoLOautiaRix_S1bjVGquh7R05RFk20BI_S9CwcaoI_P3_BEo2DHoWF9y1MBsU0y8KzcS0yNtB459WWfUMQuaOzjqYn7GwsvIv3KO25d8FbDFmG6SionYl7wAB1JoJolGnRRbgVVtDI3lri1L-dj4DS4p74hNSB5hgoYeUYy0Mk9kYXhqw-w_JihK2Mej7rrsZmqq338axdjdsL42hJYXuNUW0fyGni3vJi-TAC14bpSZZCJVMgF3tqXY6gPoK7To-gx7syy2r4eCl7gE-nRCbTTL6kVqbazQ9SnRQl7kjTL26yx92sNeIJPgC-oGQGnIYPqrx4m3Rczz-lptDizs2YWu_EQEt8eG1IVj1oV_V7JzYdvRB8Q8n3ApS3JhjRy--gPNOgrd7VaiNnsodAm8NvJ2pvtd852IvpjQwc1Mf7V3unYC6bZIRJ68OMfZSnZk_PYmAlAIGvaWNckfb-trB1KfXPjLvc2gmfIhObyYw4xyY5YoqHV9VZvJAE-meU7nK83jBIcjlNqtIV_zcrAUUbztqpPnJ54_TgMbsSTP6wfwE6gklk)

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
