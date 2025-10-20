using System;
using System.Linq;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class AsteroidFieldUseCase
    {
        private readonly IRandomProvider _rng;
        public AsteroidRules Rules { get; }

        // Rango de giro (grados/seg) para la variedad visual
        const float SPIN_MIN = 8f;
        const float SPIN_MAX = 28f;

        public AsteroidFieldUseCase(IRandomProvider rng, AsteroidRules rules)
        {
            _rng = rng;
            Rules = rules;
        }

        // ───────── Inicial: mundo “preexistente” ya poblado
        public void InitializeField(GameState state)
        {
            int target = (int)(Rules.FieldDesiredCount * Rules.InitialFillMultiplier);
            while (state.Asteroids.Count < target)
            {
                var pos = RandomPositionInShell(
                    state.Player.Position,
                    inner: MathF.Max(Rules.MinSpawnDistance, Rules.FieldRadius * 0.50f),
                    outer: Rules.FieldRadius
                );
                float r = _rng.NextFloat(Rules.MinRadius, Rules.MaxRadius);

                state.Asteroids.Add(new Asteroid
                {
                    Position = pos,
                    Radius = r,
                    Alive = true,
                    ShapeIndex = RandomShapeIndex(),
                    SpinAxis = RandomUnitVector(),
                    SpinSpeed = _rng.NextFloat(SPIN_MIN, SPIN_MAX),
                    SpinPhase = _rng.NextFloat(0f, 360f)
                });
            }
        }

        // ───────── Runtime: mantener burbuja
        public void UpdateField(GameState state)
        {
            var p = state.Player.Position;

            // 1) Despawn lejanos
            float despawn2 = Rules.DespawnRadius * Rules.DespawnRadius;
            for (int i = 0; i < state.Asteroids.Count; i++)
            {
                var a = state.Asteroids[i];
                float dx = a.Position.x - p.x;
                float dy = a.Position.y - p.y;
                float dz = a.Position.z - p.z;
                if (dx * dx + dy * dy + dz * dz > despawn2) a.Alive = false;
            }
            state.Asteroids = state.Asteroids.Where(x => x.Alive).ToList();

            // 2) Cuenta visibles
            float vis2 = Rules.FieldRadius * Rules.FieldRadius;
            int visible = 0;
            for (int i = 0; i < state.Asteroids.Count; i++)
            {
                var a = state.Asteroids[i];
                float dx = a.Position.x - p.x;
                float dy = a.Position.y - p.y;
                float dz = a.Position.z - p.z;
                if (dx * dx + dy * dy + dz * dz <= vis2) visible++;
            }

            // 3) Objetivo
            int targetVisible = (int)(Rules.FieldDesiredCount * Rules.RuntimeFillMultiplier);

            // 4) Reponer
            while (visible < targetVisible)
            {
                var pos = RandomPositionOnPerimeter(p, Rules.FieldRadius);

                float dx = pos.x - p.x, dy = pos.y - p.y, dz = pos.z - p.z;
                float d = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
                if (d < Rules.MinSpawnDistance) continue;

                float r = _rng.NextFloat(Rules.MinRadius, Rules.MaxRadius);
                state.Asteroids.Add(new Asteroid
                {
                    Position = pos,
                    Radius = r,
                    Alive = true,
                    ShapeIndex = RandomShapeIndex(),
                    SpinAxis = RandomUnitVector(),
                    SpinSpeed = _rng.NextFloat(SPIN_MIN, SPIN_MAX),
                    SpinPhase = _rng.NextFloat(0f, 360f)
                });
                visible++;
            }
        }

        // ───────── utilidades ─────────

        private Vec3 RandomUnitVector()
        {
            float u = _rng.NextFloat(-1f, 1f);
            float theta = _rng.NextFloat(0f, (float)(2 * Math.PI));
            float s = MathF.Sqrt(1 - u * u);
            return new Vec3(s * MathF.Cos(theta), u, s * MathF.Sin(theta));
        }

        private int RandomShapeIndex()
        {
            // Cualquier entero sirve; en Presentation se hace modulo por Count
            // Aquí usamos un rango razonable para variación
            return (int)_rng.NextFloat(0f, 1000f);
        }

        private Vec3 RandomPositionOnPerimeter(Vec3 center, float radius)
        {
            float u = _rng.NextFloat(-1f, 1f);
            float theta = _rng.NextFloat(0f, (float)(2 * Math.PI));
            float s = MathF.Sqrt(1 - u * u);
            float dx = s * MathF.Cos(theta);
            float dy = u;
            float dz = s * MathF.Sin(theta);
            return new Vec3(center.x + dx * radius, center.y + dy * radius, center.z + dz * radius);
        }

        private Vec3 RandomPositionInShell(Vec3 center, float inner, float outer)
        {
            float u = _rng.NextFloat(-1f, 1f);
            float theta = _rng.NextFloat(0f, (float)(2 * Math.PI));
            float s = MathF.Sqrt(1 - u * u);
            float dx = s * MathF.Cos(theta);
            float dy = u;
            float dz = s * MathF.Sin(theta);

            float dist = _rng.NextFloat(inner, outer);
            return new Vec3(center.x + dx * dist, center.y + dy * dist, center.z + dz * dist);
        }
    }
}
