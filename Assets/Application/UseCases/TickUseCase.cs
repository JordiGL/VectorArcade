using System.Linq;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class TickUseCase
    {
        private readonly ITimeProvider _time;
        private readonly WeaponRules _weaponRules;
        private readonly ScoreRules _scoreRules; // ← NUEVO

        public TickUseCase(ITimeProvider time, WeaponRules weaponRules, ScoreRules scoreRules) // ← ctor actualizado
        {
            _time = time;
            _weaponRules = weaponRules;
            _scoreRules = scoreRules;
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
                        state.Score += _scoreRules.AsteroidDestroyed; // ← antes 10
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

                        if (_scoreRules.ItemPickup != 0)
                            state.Score += _scoreRules.ItemPickup;

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
                                state.Score += _scoreRules.AsteroidDestroyed; // ← antes 10
                            }
                        }
                        m.Alive = false;
                        break;
                    }
                }
            }

            // Colisiones misil-PLANETA (solo misil lo destruye)
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                if (!m.Alive) continue;

                for (int j = 0; j < state.Planets.Count; j++)
                {
                    var pl = state.Planets[j];
                    if (!pl.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(m.Position, pl.Position, pl.Radius))
                    {
                        pl.Alive = false;
                        m.Alive = false;
                        state.Score += _scoreRules.PlanetDestroyed; // ← antes 100
                        break;
                    }
                }
            }

            // ───────── Balas vs Cometas (daños por impactos)
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Comets.Count; j++)
                {
                    var c = state.Comets[j];
                    if (!c.Alive) continue;

                    if (Domain.Physics.Collision.PointInSphere(b.Position, c.Position, c.Radius))
                    {
                        b.Alive = false;
                        c.HitsToKill--;
                        if (c.HitsToKill <= 0)
                        {
                            c.Alive = false;
                            state.Score += _scoreRules.CometDestroyed; // ← usa ScoreRules
                        }
                        break;
                    }
                }
            }

            // ───────── Misiles vs Cometas (un impacto los destruye)
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                if (!m.Alive) continue;

                for (int j = 0; j < state.Comets.Count; j++)
                {
                    var c = state.Comets[j];
                    if (!c.Alive) continue;

                    if (Domain.Physics.Collision.PointInSphere(m.Position, c.Position, c.Radius))
                    {
                        c.Alive = false;
                        m.Alive = false;
                        state.Score += _scoreRules.CometDestroyed;
                        break;
                    }
                }
            }

            // ───────── Cometas vs Jugador (aplican daño y desaparecen)
            for (int j = 0; j < state.Comets.Count; j++)
            {
                var c = state.Comets[j];
                if (!c.Alive) continue;

                if (Domain.Physics.Collision.PointInSphere(state.Player.Position, c.Position, c.Radius))
                {
                    c.Alive = false;
                    // Primero consume escudo en porcentaje; cuando llega a 0, los siguientes golpes restan vidas.
                    if (state.Player.ShieldPercent > 0)
                    {
                        int dmg = (state.CometRules != null) ? state.CometRules.DamageToShieldPercent : 25;
                        state.Player.ShieldPercent -= dmg;
                        if (state.Player.ShieldPercent < 0) state.Player.ShieldPercent = 0;
                    }
                    else
                    {
                        state.Player.Lives -= 1;
                        if (state.Player.Lives <= 0)
                        {
                            state.Player.Lives = 0;
                            state.GameOver = true;
                        }
                    }
                }
            }

            // Compactar colecciones
            state.Asteroids.RemoveAll(a => !a.Alive);
            state.Bullets.RemoveAll(b => !b.Alive);
            state.Missiles.RemoveAll(m => !m.Alive);
            state.Planets.RemoveAll(pl => !pl.Alive);
            state.Comets.RemoveAll(c => !c.Alive);
        }
    }
}

