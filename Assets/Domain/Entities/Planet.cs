using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class Planet
    {
        public Vec3 Position;
        public float Radius;
        public bool Alive = true;
    }
}
