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
        private readonly ScoreRules _scoreRules;

        public TickUseCase(ITimeProvider time, WeaponRules weaponRules, ScoreRules scoreRules)
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
                b.Position = new VectorArcade.Domain.Core.Vec3(
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
                m.Position = new VectorArcade.Domain.Core.Vec3(
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

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(b.Position, a.Position, a.Radius))
                    {
                        a.Alive = false;
                        b.Alive = false;
                        state.Score += _scoreRules.AsteroidDestroyed;
                        break;
                    }
                }
            }

            // Colisiones bala-PlanetDebris (mismo comportamiento y puntos que asteroides)
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.PlanetDebris.Count; j++)
                {
                    var d = state.PlanetDebris[j];
                    if (!d.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(b.Position, d.Position, d.Radius))
                    {
                        d.Alive = false;
                        b.Alive = false;
                        state.Score += _scoreRules.AsteroidDestroyed;
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

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(b.Position, it.Position, it.Radius))
                    {
                        it.Alive = false;
                        b.Alive = false;

                        state.Player.CurrentWeapon = WeaponType.Missile;
                        state.Player.MissilesLeft += _weaponRules.MissilesPerPickup;

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

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(m.Position, a.Position, a.Radius))
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
                                state.Score += _scoreRules.AsteroidDestroyed;
                            }
                        }
                        m.Alive = false;
                        break;
                    }
                }
            }

            // Colisiones misil-PlanetDebris (explosión en área como asteroides)
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                if (!m.Alive) continue;

                for (int j = 0; j < state.PlanetDebris.Count; j++)
                {
                    var d0 = state.PlanetDebris[j];
                    if (!d0.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(m.Position, d0.Position, d0.Radius))
                    {
                        float r = m.ExplosionRadius;
                        float r2 = r * r;
                        for (int k = 0; k < state.PlanetDebris.Count; k++)
                        {
                            var dk = state.PlanetDebris[k];
                            if (!dk.Alive) continue;

                            float dx = dk.Position.x - m.Position.x;
                            float dy = dk.Position.y - m.Position.y;
                            float dz = dk.Position.z - m.Position.z;

                            if (dx * dx + dy * dy + dz * dz <= r2)
                            {
                                dk.Alive = false;
                                state.Score += _scoreRules.AsteroidDestroyed;
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
                        state.Score += _scoreRules.PlanetDestroyed;
                        // Generar restos de planeta (aleatorios en dirección, velocidad, tamaño y spin)
                        int pieces = 24; float baseSpeed = 18f; float life = 6f;
                        for (int k = 0; k < pieces; k++)
                        {
                            // pseudo-aleatorio determinista basado en índice y posición del planeta
                            float r1 = Hash01(k * 12.9898f + pl.Position.x * 78.233f + pl.Position.y);
                            float r2 = Hash01(k * 95.233f + pl.Position.z * 37.719f + pl.Position.x);
                            float r3 = Hash01(k * 41.31f + pl.Position.y * 17.17f + pl.Position.z);

                            double uu = r1 * 2.0 - 1.0; // [-1,1]
                            double theta = r2 * (2.0 * System.Math.PI);
                            double ss = System.Math.Sqrt(System.Math.Max(0.0, 1.0 - uu * uu));
                            var dir = new VectorArcade.Domain.Core.Vec3((float)(ss * System.Math.Cos(theta)), (float)uu, (float)(ss * System.Math.Sin(theta)));

                            float spMul = 0.6f + 0.8f * r3; // 0.6..1.4
                            var vel = new VectorArcade.Domain.Core.Vec3(dir.x * (baseSpeed * spMul), dir.y * (baseSpeed * spMul), dir.z * (baseSpeed * spMul));

                            int shape = (int)(r1 * 8.0f) % 8;
                            var spinAxis = new VectorArcade.Domain.Core.Vec3(dir.y, -dir.x, dir.z);
                            float spinSpeed = 30f + 60f * r2; // 30..90
                            float spinPhase = r3 * 360f;
                            float radius = 0.4f + (0.15f + 0.12f * r2) * pl.Radius;

                            state.PlanetDebris.Add(new PlanetDebris { Position = pl.Position, Velocity = vel, Life = life, MaxLife = life, Size = 1.2f + pl.Radius * 0.15f, Alive = true, ShapeIndex = shape, SpinAxis = spinAxis, SpinSpeed = spinSpeed, SpinPhase = spinPhase, Radius = radius });
                        }
                        break;
                    }
                }
            }

            // Balas vs Cometas (daños por impactos)
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Comets.Count; j++)
                {
                    var c = state.Comets[j];
                    if (!c.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(b.Position, c.Position, c.Radius))
                    {
                        b.Alive = false;
                        c.HitsToKill--;
                        if (c.HitsToKill <= 0)
                        {
                            c.Alive = false;
                            state.Score += _scoreRules.CometDestroyed;
                        }
                        break;
                    }
                }
            }

            // Misiles vs Cometas (un impacto los destruye)
            for (int i = 0; i < state.Missiles.Count; i++)
            {
                var m = state.Missiles[i];
                if (!m.Alive) continue;

                for (int j = 0; j < state.Comets.Count; j++)
                {
                    var c = state.Comets[j];
                    if (!c.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(m.Position, c.Position, c.Radius))
                    {
                        c.Alive = false;
                        m.Alive = false;
                        state.Score += _scoreRules.CometDestroyed;
                        break;
                    }
                }
            }

            // Cometas vs Jugador (aplican daño y desaparecen)
            for (int j = 0; j < state.Comets.Count; j++)
            {
                var c = state.Comets[j];
                if (!c.Alive) continue;

                if (VectorArcade.Domain.Physics.Collision.PointInSphere(state.Player.Position, c.Position, c.Radius))
                {
                    c.Alive = false;
                    // Primero consume escudo; luego vidas
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

            // Colisiones bala-PLANETA (la bala impacta y se destruye; el planeta permanece)
            for (int i = 0; i < state.Bullets.Count; i++)
            {
                var b = state.Bullets[i];
                if (!b.Alive) continue;

                for (int j = 0; j < state.Planets.Count; j++)
                {
                    var pl = state.Planets[j];
                    if (!pl.Alive) continue;

                    if (VectorArcade.Domain.Physics.Collision.PointInSphere(b.Position, pl.Position, pl.Radius))
                    {
                        b.Alive = false;
                        break;
                    }
                }
            }

            // Compactar colecciones y actualizar restos de planeta
            state.Planets.RemoveAll(pl => !pl.Alive);

            for (int i = 0; i < state.PlanetDebris.Count; i++)
            {
                var dbr = state.PlanetDebris[i];
                dbr.Position = new VectorArcade.Domain.Core.Vec3(
                    dbr.Position.x + dbr.Velocity.x * dt,
                    dbr.Position.y + dbr.Velocity.y * dt,
                    dbr.Position.z + dbr.Velocity.z * dt
                );
                dbr.Velocity = new VectorArcade.Domain.Core.Vec3(dbr.Velocity.x * 0.995f, dbr.Velocity.y * 0.995f, dbr.Velocity.z * 0.995f);

                var ppos = state.Player.Position;
                float dx = dbr.Position.x - ppos.x;
                float dy = dbr.Position.y - ppos.y;
                float dz = dbr.Position.z - ppos.z;
                float desp = state.AsteroidRules != null ? state.AsteroidRules.DespawnRadius : 340f;
                if (dx * dx + dy * dy + dz * dz > desp * desp) dbr.Alive = false;
            }
            state.PlanetDebris.RemoveAll(d => !d.Alive);
            state.Comets.RemoveAll(c => !c.Alive);
        }

        static float Frac(float x) => x - (float)System.Math.Floor(x);
        static float Hash01(float x)
        {
            // hash rápido y determinista a [0,1)
            float s = System.MathF.Sin(x) * 43758.5453f;
            return Frac(s);
        }
    }
}
