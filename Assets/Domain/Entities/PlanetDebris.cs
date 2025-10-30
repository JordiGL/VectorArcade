// Domain/Entities/PlanetDebris.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class PlanetDebris
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public float Life;      // tiempo restante en segundos
        public float MaxLife;   // para calcular alpha si se quiere
        public float Size;      // longitud del trazo
        public bool Alive = true;

        // Apariencia tipo asteroide
        public int ShapeIndex = 0;     // índice en WireAsteroidShapes
        public Vec3 SpinAxis;          // eje de giro
        public float SpinSpeed = 30f;  // grados/seg
        public float SpinPhase = 0f;   // grados iniciales
        public float Radius = 1f;      // tamaño base del fragmento
    }
}
