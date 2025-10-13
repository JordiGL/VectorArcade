// Assets/Domain/Services/WeaponRules.cs
namespace VectorArcade.Domain.Services
{
    public sealed class WeaponRules
    {
        // Blaster
        public float BulletSpeed = 100f;
        public float BulletLife = 4.0f;
        public float FireRatePerSecond = 5.5f;
        public float MuzzleOffset = 1.5f;

        // Misil
        public float MissileSpeed = 70f;
        public float MissileLife = 6.0f;
        public float MissileFireRatePerSec = 1.8f;
        public float MissileExplosionRadius = 12f;

        // Power-up
        public int MissilesPerPickup = 4;
    }
}
