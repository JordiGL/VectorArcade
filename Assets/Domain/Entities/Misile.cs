namespace VectorArcade.Domain.Entities
{
    public sealed class Missile
    {
        public Core.Vec3 Position;
        public Core.Vec3 Velocity;
        public float Life;
        public bool Alive;

        // Para debug/efectos
        public float ExplosionRadius;
    }
}
