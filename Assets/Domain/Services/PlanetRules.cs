namespace VectorArcade.Domain.Services
{
    /// <summary>Reglas de poblamiento para Planets (pocos, grandes, lejanos).</summary>
    public sealed class PlanetRules
    {
        public int FieldDesiredCount = 4;
        public float FieldRadius = 480f;   // burbuja “visible”
        public float DespawnRadius = 560f;   // limpieza lejana
        public float MinSpawnDistance = 140f;   // evita cerca del jugador

        public float MinRadius = 8f;     // planetas más grandes
        public float MaxRadius = 18f;

        public float InitialFillMultiplier = 1.0f; // poblar al iniciar = objetivo
        public float RuntimeFillMultiplier = 1.0f; // reponer manteniendo el objetivo
    }
}
