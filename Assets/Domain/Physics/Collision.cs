// Assets/Domain/Physics/Collision.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Physics
{
    public static class Collision
    {
        // Distancia cuadrada punto-esfera (usamos esfera por simplicidad inicial)
        public static bool PointInSphere(Vec3 point, Vec3 center, float radius)
        {
            float dx = point.x - center.x;
            float dy = point.y - center.y;
            float dz = point.z - center.z;
            float d2 = dx*dx + dy*dy + dz*dz;
            return d2 <= radius * radius;
        }
    }
}
