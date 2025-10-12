// Assets/Application/UseCases/AsteroidFieldUseCase.cs
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
        public SpawnerRules Rules { get; }

        public AsteroidFieldUseCase(IRandomProvider rng, SpawnerRules rules)
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
                // Llenamos un CASCARÓN esférico amplio: lejos pero alrededor
                var pos = RandomPositionInShell(
                    state.Player.Position,
                    inner: MathF.Max(Rules.MinSpawnDistance, Rules.FieldRadius * 0.50f), // evita cerca
                    outer: Rules.FieldRadius                                          // borde vivo
                );
                float r = _rng.NextFloat(Rules.MinRadius, Rules.MaxRadius);
                state.Asteroids.Add(new Asteroid { Position = pos, Radius = r, Alive = true });
            }
        }

        // ───────── Runtime: mantener burbuja
        public void UpdateField(GameState state)
        {
            var p = state.Player.Position;

            // 1) Despawn lejanos (fuera de la burbuja externa)
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

            // 2) Cuenta SOLO los "visibles" (dentro del FieldRadius)
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

            // 3) Objetivo de visibles en runtime
            int targetVisible = (int)(Rules.FieldDesiredCount * Rules.RuntimeFillMultiplier);

            // 4) Reponer en el PERÍMETRO hasta alcanzar el objetivo visible
            while (visible < targetVisible)
            {
                var pos = RandomPositionOnPerimeter(p, Rules.FieldRadius);

                // Evitar spawns pegados (por si cambias radios en inspector)
                float dx = pos.x - p.x, dy = pos.y - p.y, dz = pos.z - p.z;
                float d = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
                if (d < Rules.MinSpawnDistance) continue;

                float r = _rng.NextFloat(Rules.MinRadius, Rules.MaxRadius);
                state.Asteroids.Add(new Asteroid { Position = pos, Radius = r, Alive = true });
                visible++; // cuenta porque nace en el perímetro (dentro de FieldRadius)
            }
        }


        // ───────── utilidades ─────────

        private Vec3 RandomPositionOnPerimeter(Vec3 center, float radius)
        {
            // dirección aleatoria uniforme
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

        private float Distance(Vec3 a, Vec3 b)
        {
            float dx = a.x - b.x, dy = a.y - b.y, dz = a.z - b.z;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
