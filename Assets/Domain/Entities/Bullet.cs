// Assets/Domain/Entities/Bullet.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class Bullet
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public float Life; // tiempo restante
        public bool Alive = true;
    }
}
