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
            if (!_input.FirePressed) return;
            if (state.Player.ShootCooldown > 0f) return;

            // Cooldown desde cadencia: 1 / dps
            float interval = (_rules.FireRatePerSecond > 0f) ? (1f / _rules.FireRatePerSecond) : 0.2f;
            state.Player.ShootCooldown = interval;

            // Crear bala desde el muzzle (delante de la cámara)
            Vec3 origin  = state.Player.Position + state.Player.Forward * _rules.MuzzleOffset;
            Vec3 vel     = state.Player.Forward * _rules.BulletSpeed;

            state.Bullets.Add(new Bullet
            {
                Position = origin,
                Velocity = vel,
                Life     = _rules.BulletLife,
                Alive    = true
            });
        }
    }
}
