using System.Linq;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Physics;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class TickUseCase
    {
        private readonly ITimeProvider _time;

        public TickUseCase(ITimeProvider time) { _time = time; }

        public void Execute(GameState state)
        {
            float dt = _time.DeltaTime;
            state.TimeSinceStart += dt;

            // Cooldown de disparo
            if (state.Player.ShootCooldown > 0f)
                state.Player.ShootCooldown -= dt;

            // Mover balas y decrementar vida
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                b.Position = new Domain.Core.Vec3(
                    b.Position.x + b.Velocity.x * dt,
                    b.Position.y + b.Velocity.y * dt,
                    b.Position.z + b.Velocity.z * dt
                );
                b.Life -= dt;
                if (b.Life <= 0f) b.Alive = false;
            }

            // Colisiones bullet-asteroid
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Asteroids.Count; j++)
                {
                    var a = state.Asteroids[j];
                    if (!a.Alive) continue;

                    if (Collision.PointInSphere(b.Position, a.Position, a.Radius))
                    {
                        a.Alive = false;
                        b.Alive = false;
                        state.Score += 10;
                        break;
                    }
                }
            }

            // Compactar
            state.Asteroids = state.Asteroids.Where(x => x.Alive).ToList();
            state.Bullets   = state.Bullets.Where(x => x.Alive).ToList();
        }
    }
}
