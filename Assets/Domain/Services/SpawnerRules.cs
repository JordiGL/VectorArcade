// Assets/Domain/Services/SpawnerRules.cs
namespace VectorArcade.Domain.Services
{
    public sealed class SpawnerRules
    {
        // Población objetivo en runtime
        public int FieldDesiredCount = 180;   // antes 80
        public float FieldRadius = 280f;  // radio vivo alrededor del jugador
        public float DespawnRadius = 340f;  // si pasan de aquí, se eliminan
        public float MinSpawnDistance = 60f;   // evita “en tu cara”

        // Tamaño de cada asteroide
        public float MinRadius = 0.6f;
        public float MaxRadius = 2.2f;

        // Boost inicial (llena el mundo “preexistente”)
        public float InitialFillMultiplier = 1.8f; // p.ej. 180 * 1.8 ≈ 324

        public float RuntimeFillMultiplier = 1.8f; // mantén la misma densidad que te gusta en juego
    }
}
