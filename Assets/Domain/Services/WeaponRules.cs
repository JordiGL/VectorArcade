// Assets/Domain/Services/WeaponRules.cs
namespace VectorArcade.Domain.Services
{
    public sealed class WeaponRules
    {
        // Velocidad de la bala (u/s) y vida (s) → alcance ≈ speed * life
        public float BulletSpeed = 100f;
        public float BulletLife  = 4.0f;

        // Cadencia en disparos por segundo (p. ej. 5.5 ≈ cada ~0.18s)
        public float FireRatePerSecond = 5.5f;

        // Desfase del cañón desde la cámara, en unidades
        public float MuzzleOffset = 1.5f;
    }
}
