// Domain/Services/CometRules.cs
namespace VectorArcade.Domain.Services
{
    /// <summary>Reglas de aparición y comportamiento para cometas (amenaza activa).</summary>
    public sealed class CometRules
    {
        // Spawning
        public float SpawnInterval = 8f;  // s (se puede reducir con dificultad)
        public float PerimeterRadius = 360f;
        public float SpawnConeDegrees = 20f; // Angulo (grados) del cono de aparici�n frente al jugador

        // Movimiento
        public float SpeedStart = 18f;    // u/s al aparecer (lejos)
        public float SpeedEnd = 8f;       // u/s cerca del jugador
        public float AimLead = 1f;        // apunta un poco por delante del jugador

        // Combate
        public int HitsToKill = 2;        // impactos de láser necesarios
        public float Radius = 1.2f;       // colisión
        public int DamageToShieldPercent = 25; // % de escudo por impacto

        // Score
        public int ScoreOnKill = 25;
    }
}

