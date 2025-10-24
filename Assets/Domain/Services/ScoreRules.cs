namespace VectorArcade.Domain.Services
{
    /// <summary>Reglas de puntuación (balance del juego).</summary>
    public sealed class ScoreRules
    {
        /// <summary>Puntos por destruir un asteroide.</summary>
        public int AsteroidDestroyed = 10;

        /// <summary>Puntos por destruir un planeta.</summary>
        public int PlanetDestroyed = 100;

        /// <summary>Puntos por recoger un item (si aplicase puntuación).</summary>
        public int ItemPickup = 0;

        /// <summary>Puntos por destruir un cometa.</summary>
        public int CometDestroyed = 25;
    }
}
