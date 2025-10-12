// Assets/Domain/Core/Vec.cs
namespace VectorArcade.Domain.Core
{
    public readonly struct Vec3
    {
        public readonly float x, y, z;
        public Vec3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

        public static Vec3 Zero => new(0, 0, 0);
        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vec3 operator *(Vec3 a, float s) => new(a.x * s, a.y * s, a.z * s);
    }
}
