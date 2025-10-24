// Domain/Services/CometRules.cs
namespace VectorArcade.Domain.Services
{
    /// <summary>Reglas de aparición y comportamiento para cometas (amenaza activa).</summary>
    public sealed class CometRules
    {
        // Spawning
        public float SpawnInterval = 1f;  // s (se puede reducir con dificultad)
        public float PerimeterRadius = 360f;

        // Movimiento
        public float Speed = 38f;         // u/s
        public float AimLead = 1f;       // apunta un poco por delante del jugador

        // Combate
        public int HitsToKill = 2;        // impactos de láser necesarios
        public float Radius = 1.2f;       // colisión
        public int DamageToShieldPercent = 25; // % de escudo por impacto

        // Score
        public int ScoreOnKill = 25;
    }
}
