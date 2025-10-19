using NUnit.Framework;
using VectorArcade.Application.UseCases;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;
using VectorArcade.Tests.Fakes;

/// <summary>
/// Conjunto de pruebas unitarias que validan el comportamiento del caso de uso <see cref="PlayerControlUseCase"/>.
/// 
/// Este caso de uso gestiona la orientación (yaw/pitch) y el avance del jugador en el espacio 3D
/// a partir de la entrada del ratón y del tiempo transcurrido.
/// 
/// Las pruebas verifican:
/// 1. Que el jugador avanza a velocidad constante en la dirección +Z mundial cuando no hay entrada de ratón.
/// 2. Que las variaciones de yaw/pitch modifican correctamente el vector Forward,
///    manteniendo un límite de inclinación vertical (±80°) para evitar rotaciones extremas.
/// </summary>
public class PlayerControlUseCase_Tests
{
    /// <summary>
    /// Verifica que, sin movimiento de ratón, el jugador avanza exactamente hacia adelante (+Z)
    /// durante 1 segundo simulado, con la velocidad de avance fija definida dentro del caso de uso.
    /// 
    /// Condiciones de éxito:
    /// - La posición Z aumenta ≈18 unidades tras 1s (FORWARD_SPEED = 18f).
    /// - La dirección Forward permanece apuntando hacia +Z.
    /// - No se introducen desviaciones en X o Y.
    /// </summary>
    [Test]
    public void MovesForward_1Second_NoMouseDelta()
    {
        var time = new FakeTime { DeltaTime = 1f / 60f, Time = 0f };
        var input = new FakeInput { MouseDeltaX = 0f, MouseDeltaY = 0f };

        var state = new GameState
        {
            Player = new Player
            {
                Position = new Vec3(0, 0, 0),
                Forward = new Vec3(0, 0, 1),
                Speed = 12f,     // no interviene aquí, el UC usa FORWARD_SPEED interno=18
                ShootRate = 0.2f
            }
        };

        var uc = new PlayerControlUseCase(input, time);

        // 60 ticks = ~1s simulado. FORWARD_SPEED = 18 u/s → z ≈ 18
        for (int i = 0; i < 60; i++) uc.Execute(state);

        Assert.That(state.Player.Position.z, Is.EqualTo(18f).Within(0.05f));
        Assert.That(state.Player.Position.x, Is.EqualTo(0f).Within(1e-4f));
        Assert.That(state.Player.Position.y, Is.EqualTo(0f).Within(1e-4f));
        // Con ratón quieto, el forward se mantiene mirando +Z
        Assert.That(state.Player.Forward.z, Is.GreaterThan(0.99f));
        Assert.That(state.Player.Forward.x, Is.EqualTo(0f).Within(1e-3f));
        Assert.That(state.Player.Forward.y, Is.EqualTo(0f).Within(1e-3f));
    }

    /// <summary>
    /// Verifica que los movimientos del ratón afectan correctamente la orientación del jugador:
    /// 
    /// - El yaw (MouseDeltaX) genera rotación horizontal, modificando la componente X del vector Forward.
    /// - El pitch (MouseDeltaY) modifica la inclinación vertical (componente Y),
    ///   pero se limita mediante un clamp a ±80° para evitar giros excesivos.
    /// - El vector Forward mantiene longitud unitaria tras múltiples actualizaciones.
    /// </summary>
    [Test]
    public void YawPitch_AffectForward_ClampPitch()
    {
        var time = new FakeTime { DeltaTime = 1f / 60f, Time = 0f };
        var input = new FakeInput();

        var state = new GameState
        {
            Player = new Player
            {
                Position = new Vec3(0, 0, 0),
                Forward = new Vec3(0, 0, 1)
            }
        };

        var uc = new PlayerControlUseCase(input, time);

        // Aplica deltas de ratón durante 60 frames (~1s)
        // Yaw positivo → mira hacia +X; Pitch muy grande → se clamp a ±80°
        input.MouseDeltaX = 5f;   // yaw creciente
        input.MouseDeltaY = -100f; // pitch arriba (con inversión en UC), forzará clamp a +80°
        for (int i = 0; i < 60; i++) uc.Execute(state);

        var f = state.Player.Forward;

        // Debe haber componente X por el yaw
        Assert.That(System.Math.Abs(f.x), Is.GreaterThan(0.1f));
        // Pitch clampado → componente Y no debería superar sin control: |sin(80°)| ~ 0.985
        Assert.That(System.Math.Abs(f.y), Is.LessThanOrEqualTo(0.99f));
        // Forward debe seguir siendo un vector no nulo
        var len = System.Math.Sqrt(f.x * f.x + f.y * f.y + f.z * f.z);
        Assert.That(len, Is.InRange(0.99, 1.01));
    }
}
