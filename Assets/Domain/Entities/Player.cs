// Assets/Domain/Entities/Player.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public enum WeaponType { Blaster = 0, Missile = 1 }

    public sealed class Player
    {
        public Vec3 Position;
        public Vec3 Forward;   // dirección de avance (normalizada en lógica)
        public float Speed;    // unidades/seg
        public float ShootRate;     // seg entre disparos

        // Cooldowns independientes
        public float ShootCooldownPrimary;   // láser
        public float ShootCooldownSecondary; // misil

        // Power-up de misil por tiempo (0 = infinito si así lo decides en rules)
        public WeaponType CurrentWeapon = WeaponType.Blaster; // informativo
        public int MissilesLeft = 0;
    }
}
