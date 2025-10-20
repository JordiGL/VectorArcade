using NUnit.Framework;
using VectorArcade.Application.UseCases;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;
using VectorArcade.Tests.Fakes;

/// <summary>
/// Pruebas unitarias para el caso de uso <see cref="TickUseCase"/>, encargado de
/// actualizar el estado general del juego (tiempo, balas, misiles, asteroides, puntuación…)
/// en cada ciclo lógico.
///
/// Este caso de uso representa el “tick” principal de simulación: reduce cooldowns,
/// mueve proyectiles, aplica colisiones y elimina entidades muertas.
/// 
/// Las pruebas aquí definidas validan que:
/// 1. Los cooldowns de disparo del jugador se reducen correctamente en función del deltaTime.
/// 2. Las balas decrementan su tiempo de vida (Life) y son eliminadas cuando éste llega a cero.
/// 3. La compactación de listas (RemoveAll) limpia correctamente los proyectiles inactivos.
/// </summary>
public class TickUseCase_Should_UpdateCooldowns
{
    /// <summary>
    /// Comprueba que <see cref="TickUseCase.Execute"/>:
    /// - Decrementa los cooldowns primario y secundario del jugador en función del deltaTime.
    /// - Reduce la vida de las balas activas hasta que expiran.
    /// - Elimina (compacta) las balas muertas de la lista tras el segundo tick.
    ///
    /// Escenario simulado:
    /// - DeltaTime = 0.5 s → se ejecutan dos ciclos (1 s total).
    /// - El cooldown primario pasa de 1.0 s a 0 s.
    /// - El cooldown secundario pasa de 2.0 s a 1.0 s.
    /// - La bala con Life=1.0 s muere y la lista queda vacía tras la compactación.
    /// </summary>
    [Test]
    public void Decrements_Cooldowns_And_BulletLife()
    {
        var time = new FakeTime { DeltaTime = 0.5f, Time = 0f };

        // Reglas de armas de ejemplo (valores por defecto razonables)
        var weaponRules = new WeaponRules
        {
            BulletLife = 2.0f,
            FireRatePerSecond = 5.0f,
            BulletSpeed = 100f,
            MuzzleOffset = 1.0f,
            MissilesPerPickup = 3
        };

        var state = new GameState
        {
            Player = new Player
            {
                Position = new Vec3(0, 0, 0),
                Forward = new Vec3(0, 0, 1),
                Speed = 12f,
                ShootRate = 0.2f,
                ShootCooldownPrimary = 1.0f,     // 1 s
                ShootCooldownSecondary = 2.0f    // 2 s
            }
        };

        // Una bala viva para comprobar decremento de vida
        state.Bullets.Add(new Bullet
        {
            Position = new Vec3(0, 0, 1),
            Velocity = new Vec3(0, 0, 10),
            Life = 1.0f,
            Alive = true
        });
        var score = new ScoreRules { AsteroidDestroyed = 10, PlanetDestroyed = 100, ItemPickup = 0 };
        var tick = new TickUseCase(time, weaponRules, score);

        // Ejecuta 2 ticks de 0.5 s = 1.0 s total
        tick.Execute(state);
        tick.Execute(state);

        // Validaciones
        Assert.That(state.Player.ShootCooldownPrimary, Is.EqualTo(0f).Within(1e-5),
            "Primary CD debería llegar a 0 tras 1 s.");
        Assert.That(state.Player.ShootCooldownSecondary, Is.EqualTo(1.0f).Within(1e-5),
            "Secondary CD debería reducirse a 1.0 s tras 1 s total.");
        // La bala muere y la lista queda vacía por compactación
        Assert.That(state.Bullets.Count, Is.EqualTo(0),
            "Las balas muertas deben eliminarse durante la compactación.");
    }
}
