// Assets/Domain/Entities/Asteroid.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class Asteroid
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public float Radius;
        public bool Alive = true;
    }
}
