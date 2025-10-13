using System.Linq;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class ItemUseCase
    {
        private readonly ITimeProvider _time;
        private readonly IRandomProvider _rng;
        private readonly ItemRules _rules;
        private float _accum;

        public ItemUseCase(ITimeProvider time, IRandomProvider rng, ItemRules rules)
        {
            _time = time; _rng = rng; _rules = rules;
        }

        public void Execute(GameState state, WeaponRules weaponRules)
        {
            float dt = _time.DeltaTime;
            _accum += dt;

            // — Spawn — (uno cada X s; 1 simultáneo para MVP)
            if (_accum >= _rules.SpawnEverySeconds && state.Items.Count < 1)
            {
                _accum = 0f;

                var p = state.Player.Position;
                var f = Normalize(state.Player.Forward);
                var upW = new Vec3(0, 1, 0);
                var right = Normalize(Cross(upW, f));
                if (right.x * right.x + right.y * right.y + right.z * right.z < 1e-6f) right = new Vec3(1, 0, 0);
                var up = Cross(f, right);

                float offX = _rng.NextFloat(-_rules.OffsetXRange, _rules.OffsetXRange);
                float offY = _rng.NextFloat(-_rules.OffsetYRange, _rules.OffsetYRange);

                var pos = new Vec3(
                    p.x + f.x * _rules.PlaneDistance + right.x * offX + up.x * offY,
                    p.y + f.y * _rules.PlaneDistance + right.y * offX + up.y * offY,
                    p.z + f.z * _rules.PlaneDistance + right.z * offX + up.z * offY
                );

                state.Items.Add(new Item
                {
                    Position = pos,
                    Type = ItemType.MissileUpgrade,
                    Alive = true,
                    Radius = _rules.ItemRadius,
                    Life = _rules.ItemLifetime
                });
            }

            // — Update — (pequeño drift opcional hacia ti y cuenta atrás de vida)
            for (int i = 0; i < state.Items.Count; i++)
            {
                var it = state.Items[i];
                if (!it.Alive) continue;

                // drift hacia el jugador (muy sutil)
                if (_rules.DriftTowardsSpeed > 0f)
                {
                    var p = state.Player.Position;
                    var dir = Normalize(new Vec3(p.x - it.Position.x, p.y - it.Position.y, p.z - it.Position.z));
                    it.Position = new Vec3(
                        it.Position.x + dir.x * _rules.DriftTowardsSpeed * dt,
                        it.Position.y + dir.y * _rules.DriftTowardsSpeed * dt,
                        it.Position.z + dir.z * _rules.DriftTowardsSpeed * dt
                    );
                }

                it.Life -= dt;
                if (it.Life <= 0f) it.Alive = false;
            }

            state.Items = state.Items.Where(x => x.Alive).ToList();
        }

        // helpers (ya los tienes)
        private static Vec3 Cross(Vec3 a, Vec3 b) => new Vec3(
            a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x
        );
        private Vec3 Normalize(Vec3 v)
        {
            float m2 = v.x * v.x + v.y * v.y + v.z * v.z;
            if (m2 <= 1e-8f) return new Vec3(0, 0, 1);
            float inv = 1f / (float)System.Math.Sqrt(m2);
            return new Vec3(v.x * inv, v.y * inv, v.z * inv);
        }

    }
}
