namespace VectorArcade.Domain.Entities
{
    public enum ItemType { MissileUpgrade }

    public sealed class Item
    {
        public Core.Vec3 Position;
        public ItemType Type;
        public bool Alive = true;

        public float Radius = 2.0f; // para colisión por disparo
        public float Life = 15f;    // segundos antes de desaparecer
    }
}
