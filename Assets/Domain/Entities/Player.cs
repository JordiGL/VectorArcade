// Assets/Domain/Entities/Player.cs
using VectorArcade.Domain.Core;

namespace VectorArcade.Domain.Entities
{
    public sealed class Player
    {
        public Vec3 Position;
        public Vec3 Forward;   // dirección de avance (normalizada en lógica)
        public float Speed;    // unidades/seg

        public float ShootCooldown; // seg restantes para poder disparar
        public float ShootRate;     // seg entre disparos
    }
}
