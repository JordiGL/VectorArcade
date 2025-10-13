// Assets/Application/UseCases/ShootUseCase.cs
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    public sealed class ShootUseCase
    {
        private readonly IInputProvider _input;
        private readonly WeaponRules _rules;

        public ShootUseCase(IInputProvider input, WeaponRules rules)
        {
            _input = input;
            _rules = rules;
        }

        public void Execute(GameState state)
        {
            // Láser (primario)
            if (state.Player.ShootCooldownPrimary <= 0f && _input.FirePrimary)
                FireBlaster(state);

            // Misil (secundario): solo si hay misiles disponibles
            if (state.Player.MissilesLeft > 0 &&
                state.Player.ShootCooldownSecondary <= 0f &&
                _input.FireSecondary)
            {
                FireMissile(state);
                state.Player.MissilesLeft--; // ← consume uno
                if (state.Player.MissilesLeft <= 0)
                    state.Player.CurrentWeapon = WeaponType.Blaster; // vuelve al láser
            }
        }

        void FireBlaster(GameState state)
        {
            float interval = (_rules.FireRatePerSecond > 0f) ? (1f / _rules.FireRatePerSecond) : 0.15f;
            state.Player.ShootCooldownPrimary = interval;

            Vec3 origin = state.Player.Position + state.Player.Forward * _rules.MuzzleOffset;
            Vec3 vel = state.Player.Forward * _rules.BulletSpeed;

            state.Bullets.Add(new Bullet
            {
                Position = origin,
                Velocity = vel,
                Life = _rules.BulletLife,
                Alive = true
            });
        }

        void FireMissile(GameState state)
        {
            float interval = (_rules.MissileFireRatePerSec > 0f) ? (1f / _rules.MissileFireRatePerSec) : 0.5f;
            state.Player.ShootCooldownSecondary = interval;

            Vec3 origin = state.Player.Position + state.Player.Forward * _rules.MuzzleOffset;
            Vec3 vel = state.Player.Forward * _rules.MissileSpeed;

            state.Missiles.Add(new Missile
            {
                Position = origin,
                Velocity = vel,
                Life = _rules.MissileLife,
                Alive = true,
                ExplosionRadius = _rules.MissileExplosionRadius
            });
        }
    }
}
