using System;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    /// <summary>Spawnea cometas hacia el jugador y actualiza su movimiento.</summary>
    public sealed class CometUseCase
    {
        private readonly ITimeProvider _time;
        private readonly IRandomProvider _rng;
        private float _timer;

        public CometRules Rules { get; }

        public CometUseCase(ITimeProvider time, IRandomProvider rng, CometRules rules)
        {
            _time = time;
            _rng = rng;
            Rules = rules;
        }

        public void Execute(GameState state)
        {
            float dt = _time.DeltaTime;

            // ───────── Spawn periódico
            _timer += dt;
            if (_timer >= Rules.SpawnInterval)
            {
                _timer = 0f;
                SpawnTowardPlayer(state); // usa la versión con intercepción predictiva
            }

            // ───────── Movimiento base
            for (int i = 0; i < state.Comets.Count; i++)
            {
                var c = state.Comets[i];
                c.Position = new Vec3(
                    c.Position.x + c.Velocity.x * dt,
                    c.Position.y + c.Velocity.y * dt,
                    c.Position.z + c.Velocity.z * dt
                );
            }

            // ───────── PLUS opcional: steering suave sin Unity
            const float turnRateRad = 2.0f;      // giro máx. por segundo (pequeño)

            var playerPos = state.Player.Position;
            var playerVel = state.Player.Velocity;

            for (int i = 0; i < state.Comets.Count; i++)
            {
                var c = state.Comets[i];
                // steering continuo (sin ventana de tiempo); se desactiva solo si ya pas� al jugador

                // Intercepción predictiva desde la posición actual del cometa
                // Resolver || (P - C) + Vt * t || = s * t
                var Sr2 = new Vec3(playerPos.x - c.Position.x, playerPos.y - c.Position.y, playerPos.z - c.Position.z);
                float vt2_2 = playerVel.x * playerVel.x + playerVel.y * playerVel.y + playerVel.z * playerVel.z;
                float distNow = Length(Sr2);
                float speedNow = SpeedByDistance(distNow, Rules);
                float s2_2 = speedNow * speedNow;
                float a2 = vt2_2 - s2_2;
                float b2 = 2f * (Sr2.x * playerVel.x + Sr2.y * playerVel.y + Sr2.z * playerVel.z);
                float c2 = (Sr2.x * Sr2.x + Sr2.y * Sr2.y + Sr2.z * Sr2.z);

                float t2 = -1f;
                if (System.MathF.Abs(a2) < 1e-6f)
                {
                    if (System.MathF.Abs(b2) > 1e-6f)
                    {
                        float tlin = -c2 / b2;
                        if (tlin > 0f) t2 = tlin;
                    }
                }
                else
                {
                    float disc2 = b2 * b2 - 4f * a2 * c2;
                    if (disc2 >= 0f)
                    {
                        float sqrt2 = System.MathF.Sqrt(disc2);
                        float tt1 = (-b2 + sqrt2) / (2f * a2);
                        float tt2 = (-b2 - sqrt2) / (2f * a2);
                        if (tt1 > 0f && tt2 > 0f) t2 = System.MathF.Min(tt1, tt2);
                        else if (tt1 > 0f) t2 = tt1;
                        else if (tt2 > 0f) t2 = tt2;
                    }
                }

                Vec3 target = (t2 > 0f)
                    ? new Vec3(playerPos.x + playerVel.x * t2, playerPos.y + playerVel.y * t2, playerPos.z + playerVel.z * t2)
                    : playerPos; // si no hay solución, perseguir posición actual
                var to = new Vec3(target.x - c.Position.x, target.y - c.Position.y, target.z - c.Position.z);

                // Si ya te ha pasado: (player - comet) ⋅ velocity <= 0 → desactiva steering
                float dotAhead = (playerPos.x - c.Position.x) * c.Velocity.x
                               + (playerPos.y - c.Position.y) * c.Velocity.y
                               + (playerPos.z - c.Position.z) * c.Velocity.z;
                if (dotAhead <= 0f)
                {
                    c.CanSteer = false;
                    continue;
                }

                float toLen = Length(to);
                if (toLen < 1e-5f) continue;
                var toDir = new Vec3(to.x / toLen, to.y / toLen, to.z / toLen);

                var v = c.Velocity;
                float vLen = Length(v);
                if (vLen < 1e-5f) continue;
                var cur = new Vec3(v.x / vLen, v.y / vLen, v.z / vLen);

                // ángulo entre cur y toDir
                float dot = Clamp(cur.x * toDir.x + cur.y * toDir.y + cur.z * toDir.z, -1f, 1f);
                float angle = System.MathF.Acos(dot);

                if (angle > 1e-4f)
                {
                    float maxAngle = turnRateRad * dt;
                    float t = System.MathF.Min(1f, maxAngle / angle);

                    // interpola direcciones y renormaliza (aprox RotateTowards)
                    var newDir = new Vec3(
                        cur.x + (toDir.x - cur.x) * t,
                        cur.y + (toDir.y - cur.y) * t,
                        cur.z + (toDir.z - cur.z) * t
                    );

                    float n = Length(newDir);
                    if (n > 1e-6f)
                    {
                        newDir = new Vec3(newDir.x / n, newDir.y / n, newDir.z / n);
                        c.Velocity = new Vec3(newDir.x * speedNow, newDir.y * speedNow, newDir.z * speedNow);
                    }
                }
            }

            // ───────── Limpieza local
            state.Comets.RemoveAll(c => !c.Alive);
            // Ajustar magnitud de velocidad para todos seg�n distancia (incl. sin steering)
            for (int i = 0; i < state.Comets.Count; i++)
            {
                var cAdj = state.Comets[i];
                float dx = state.Player.Position.x - cAdj.Position.x;
                float dy = state.Player.Position.y - cAdj.Position.y;
                float dz = state.Player.Position.z - cAdj.Position.z;
                float distAdj = System.MathF.Sqrt(dx * dx + dy * dy + dz * dz);
                float spdAdj = SpeedByDistance(distAdj, Rules);
                var vv = cAdj.Velocity; float vLenAdj = Length(vv);
                if (vLenAdj > 1e-6f)
                    cAdj.Velocity = new Vec3(vv.x / vLenAdj * spdAdj, vv.y / vLenAdj * spdAdj, vv.z / vLenAdj * spdAdj);
            }
        }

        public void Reset()
        {
            _timer = 0f;
        }

        void SpawnTowardPlayer(GameState state)
        {
            // Punto de spawn: siempre delante del jugador dentro de un cono de SpawnConeDegrees
            var Ppos = state.Player.Position;
            var V = state.Player.Velocity;
            float Vlen = Length(V);
            // Si la velocidad es casi cero, usamos la mirada o +Z mundial
            Vec3 fwd = Vlen > 1e-5f ? new Vec3(V.x / Vlen, V.y / Vlen, V.z / Vlen)
                                     : (Length(state.Player.Forward) > 1e-5f ? state.Player.Forward : new Vec3(0, 0, 1));

            // Base ortonormal (right, up2, fwd)
            Vec3 refUp = new Vec3(0, 1, 0);
            float d = fwd.x * refUp.x + fwd.y * refUp.y + fwd.z * refUp.z;
            if (System.MathF.Abs(d) > 0.99f) refUp = new Vec3(1, 0, 0);
            Vec3 right = Normalize(Cross(refUp, fwd));
            Vec3 up2 = Cross(fwd, right);

            float half = Rules.SpawnConeDegrees * (float)System.Math.PI / 180f;
            float cosMin = System.MathF.Cos(half);
            float cosA = _rng.NextFloat(cosMin, 1f);
            float sinA = System.MathF.Sqrt(System.MathF.Max(0f, 1f - cosA * cosA));
            float theta = _rng.NextFloat(0f, (float)(2 * System.Math.PI));
            float ct = System.MathF.Cos(theta), st = System.MathF.Sin(theta);
            var dir = new Vec3(
                fwd.x * cosA + (right.x * ct + up2.x * st) * sinA,
                fwd.y * cosA + (right.y * ct + up2.y * st) * sinA,
                fwd.z * cosA + (right.z * ct + up2.z * st) * sinA
            );

            var spawn = new Vec3(
                Ppos.x + dir.x * Rules.PerimeterRadius,
                Ppos.y + dir.y * Rules.PerimeterRadius,
                Ppos.z + dir.z * Rules.PerimeterRadius
            );

            // Datos para intercepción
            var P = state.Player.Position;
            var Vt = state.Player.Velocity; // velocidad real del jugador (no la mirada)
            var Sr = new Vec3(P.x - spawn.x, P.y - spawn.y, P.z - spawn.z); // vector relativo (player - spawn)

            float vt2 = Vt.x * Vt.x + Vt.y * Vt.y + Vt.z * Vt.z;
            float s2 = Rules.SpeedStart * Rules.SpeedStart;

            // Resolver a t^2 + b t + c = 0
            float a = vt2 - s2;
            float b = 2f * (Sr.x * Vt.x + Sr.y * Vt.y + Sr.z * Vt.z);
            float c = (Sr.x * Sr.x + Sr.y * Sr.y + Sr.z * Sr.z);

            float tIntercept = -1f;
            if (System.MathF.Abs(a) < 1e-6f)
            {
                if (System.MathF.Abs(b) > 1e-6f)
                {
                    float t = -c / b;
                    if (t > 0f) tIntercept = t;
                }
            }
            else
            {
                float disc = b * b - 4f * a * c;
                if (disc >= 0f)
                {
                    float sqrt = System.MathF.Sqrt(disc);
                    float t1 = (-b + sqrt) / (2f * a);
                    float t2 = (-b - sqrt) / (2f * a);
                    if (t1 > 0f && t2 > 0f) tIntercept = System.MathF.Min(t1, t2);
                    else if (t1 > 0f) tIntercept = t1;
                    else if (t2 > 0f) tIntercept = t2;
                }
            }

            Vec3 target;
            if (tIntercept > 0f)
            {
                target = new Vec3(P.x + Vt.x * tIntercept, P.y + Vt.y * tIntercept, P.z + Vt.z * tIntercept);
            }
            else
            {
                // Fallback: lead fijo en dirección de movimiento; si está parado, apunta al jugador directamente
                float vlen = Length(Vt);
                if (vlen > 1e-5f)
                {
                    var dirV = new Vec3(Vt.x / vlen, Vt.y / vlen, Vt.z / vlen);
                    target = new Vec3(P.x + dirV.x * Rules.AimLead,
                                      P.y + dirV.y * Rules.AimLead,
                                      P.z + dirV.z * Rules.AimLead);
                }
                else
                {
                    target = P;
                }
            }

            // Velocidad hacia el target con módulo Speed
            float vx = target.x - spawn.x;
            float vy = target.y - spawn.y;
            float vz = target.z - spawn.z;
            float mag = System.MathF.Sqrt(vx * vx + vy * vy + vz * vz);
            if (mag < 1e-5f) mag = 1f;

            vx = vx / mag * Rules.SpeedStart;
            vy = vy / mag * Rules.SpeedStart;
            vz = vz / mag * Rules.SpeedStart;

            state.Comets.Add(new Comet
            {
                Position = spawn,
                Velocity = new Vec3(vx, vy, vz),
                Alive = true,
                HitsToKill = Rules.HitsToKill,
                Radius = Rules.Radius
            });
        }

        // Helpers matemáticos mínimos (sin Unity)
        static float Length(Vec3 v) => System.MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
        static Vec3 Cross(Vec3 a, Vec3 b) => new Vec3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
        static Vec3 Normalize(Vec3 v)
        {
            float n = Length(v);
            return n > 1e-6f ? new Vec3(v.x / n, v.y / n, v.z / n) : new Vec3(0, 0, 0);
        }

        // Velocidad deseada según distancia al jugador: lejos -> SpeedStart, cerca -> SpeedEnd
        static float Lerp(float a, float b, float t) => a + (b - a) * t;
        static float SpeedByDistance(float dist, CometRules rules)
        {
            float t = Clamp(dist / System.MathF.Max(1e-3f, rules.PerimeterRadius), 0f, 1f);
            return Lerp(rules.SpeedEnd, rules.SpeedStart, t);
        }
    }
}






