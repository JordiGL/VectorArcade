using System.Linq;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class TickUseCase
    {
        private readonly ITimeProvider _time;
        private readonly WeaponRules _weaponRules; // ← NUEVO

        public TickUseCase(ITimeProvider time, WeaponRules weaponRules) // ← ctor actualizado
        {
            _time = time;
            _weaponRules = weaponRules; // ← guarda reglas
        }
        public void Execute(GameState state)
        {
            float dt = _time.DeltaTime;
            state.TimeSinceStart += dt;

            // Cooldowns
            if (state.Player.ShootCooldownPrimary > 0f) state.Player.ShootCooldownPrimary -= dt;
            if (state.Player.ShootCooldownSecondary > 0f) state.Player.ShootCooldownSecondary -= dt;

            // Balas
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

            // Misiles
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                m.Position = new Domain.Core.Vec3(
                    m.Position.x + m.Velocity.x * dt,
                    m.Position.y + m.Velocity.y * dt,
                    m.Position.z + m.Velocity.z * dt
                );
                m.Life -= dt;
                if (m.Life <= 0f) m.Alive = false;
            }

            // Colisiones bala-asteroide
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Asteroids.Count; j++)
                {
                    var a = state.Asteroids[j];
                    if (!a.Alive) continue;

                    if (Domain.Physics.Collision.PointInSphere(b.Position, a.Position, a.Radius))
                    {
                        a.Alive = false;
                        b.Alive = false;
                        state.Score += 10;
                        break;
                    }
                }
            }

            // Colisiones bala-item (recoger disparando)
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Items.Count; j++)
                {
                    var it = state.Items[j];
                    if (!it.Alive) continue;

                    if (Domain.Physics.Collision.PointInSphere(b.Position, it.Position, it.Radius))
                    {
                        it.Alive = false;
                        b.Alive = false;

                        state.Player.CurrentWeapon = WeaponType.Missile;
                        state.Player.MissilesLeft = _weaponRules.MissilesPerPickup;
                        break;
                    }

                }
            }

            // Colisiones misil-asteroide (explosión en área)
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                if (!m.Alive) continue;

                for (int j = 0; j < state.Asteroids.Count; j++)
                {
                    var a = state.Asteroids[j];
                    if (!a.Alive) continue;

                    if (Domain.Physics.Collision.PointInSphere(m.Position, a.Position, a.Radius))
                    {
                        // Explota: elimina todos los asteroides dentro del radio
                        float r = m.ExplosionRadius;
                        float r2 = r * r;
                        for (int k = 0; k < state.Asteroids.Count; k++)
                        {
                            var ak = state.Asteroids[k];
                            if (!ak.Alive) continue;
                            float dx = ak.Position.x - m.Position.x;
                            float dy = ak.Position.y - m.Position.y;
                            float dz = ak.Position.z - m.Position.z;
                            if (dx * dx + dy * dy + dz * dz <= r2)
                            {
                                ak.Alive = false;
                                state.Score += 10;
                            }
                        }
                        m.Alive = false;
                        break;
                    }
                }
            }

            // Compactar colecciones
            state.Asteroids = state.Asteroids.Where(x => x.Alive).ToList();
            state.Bullets = state.Bullets.Where(x => x.Alive).ToList();
            state.Missiles = state.Missiles.Where(x => x.Alive).ToList();
        }
    }
}
