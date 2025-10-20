using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class Asteroid
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public float Radius;
        public bool Alive = true;

        // ───────── Visual (wireframe 3D)
        // Índice de plantilla (se resolverá en Presentation con mod del número de formas)
        public int ShapeIndex;

        // Eje de giro y velocidad (grados/seg) + fase inicial (grados)
        public Vec3 SpinAxis;   // no tiene por qué estar normalizado
        public float SpinSpeed; // deg/s
        public float SpinPhase; // deg
    }
}
