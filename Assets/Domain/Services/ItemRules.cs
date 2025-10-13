namespace VectorArcade.Domain.Services
{
    public sealed class ItemRules
    {
        public float SpawnEverySeconds = 8f;

        // Plano frontal donde aparece (distancia en tu forward)
        public float PlaneDistance = 140f;

        // Offsets aleatorios en tus ejes right/up
        public float OffsetXRange = 80f; // ±X
        public float OffsetYRange = 45f; // ±Y

        // Tamaño y vida
        public float ItemRadius = 2.0f;
        public float ItemLifetime = 15f;

        // Movimiento opcional (pequeño drift hacia ti)
        public float DriftTowardsSpeed = 6f; // 0 = sin drift
    }
}
