// Domain/Entities/Comet.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    // Domain/Entities/Comet.cs
    public sealed class Comet
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public bool Alive = true;
        public int HitsToKill = 2;
        public float Radius = 1.2f;

        // NUEVO: steering permitido solo al principio (evita órbitas)
        public bool CanSteer = true;
        public float SteerTimeLeft = 2.0f; // s, ventana corta de adquisición
    }

}
