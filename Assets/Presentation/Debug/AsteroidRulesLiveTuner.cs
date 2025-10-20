// Assets/Presentation/Debug/AsteroidRulesLiveTuner.cs
using UnityEngine;
using VectorArcade.Presentation.Bootstrap;
using VectorArcade.Domain.Services;

namespace VectorArcade.Presentation.DebugTools
{
    /// Permite ajustar AsteroidRules en caliente desde la escena.
    /// - Marca "Apply Continuously" para aplicar cada frame en Play.
    /// - Usa "Apply Now" para empujar valores manualmente.
    /// - Usa "Refill Initial Field" para reprobar el campo inicial (cuidado: puede ser costoso).
    public sealed class AsteroidRulesLiveTuner : MonoBehaviour
    {
        [Header("Scene wiring")]
        public GameInstaller installer;

        [Header("Live apply")]
        public bool applyContinuously = true;

        [Header("Population & Radii")]
        [Min(0)] public int fieldDesiredCount = 180;
        [Min(1f)] public float fieldRadius = 280f;
        [Min(1f)] public float despawnRadius = 340f;
        [Min(0f)] public float minSpawnDistance = 60f;
        [Min(0.1f)] public float runtimeFillMultiplier = 1.8f;

        [Header("Asteroid size")]
        [Min(0.1f)] public float minRadius = 0.6f;
        [Min(0.1f)] public float maxRadius = 2.2f;

        [Header("Initial Boost")]
        [Min(0.1f)] public float initialFillMultiplier = 1.8f;

        void Awake()
        {
            if (installer == null) return;
            // Cargamos valores actuales del installer → UI
            var r = installer.asteroidRules;
            fieldDesiredCount = r.FieldDesiredCount;
            fieldRadius = r.FieldRadius;
            despawnRadius = r.DespawnRadius;
            minSpawnDistance = r.MinSpawnDistance;
            minRadius = r.MinRadius;
            maxRadius = r.MaxRadius;
            initialFillMultiplier = r.InitialFillMultiplier;
            runtimeFillMultiplier = r.RuntimeFillMultiplier;
        }

        void Update()
        {
            if (!UnityEngine.Application.isPlaying || installer == null) return;
            if (applyContinuously)
                ApplyNow();
        }

        [ContextMenu("Apply Now")]
        public void ApplyNow()
        {
            if (installer == null) return;

            // Sanitizar y aplicar al objeto de reglas compartido
            var r = installer.asteroidRules;

            if (maxRadius < minRadius) maxRadius = minRadius;

            r.RuntimeFillMultiplier = Mathf.Max(0.1f, runtimeFillMultiplier);
            r.FieldDesiredCount = Mathf.Max(0, fieldDesiredCount);
            r.FieldRadius = Mathf.Max(1f, fieldRadius);
            r.DespawnRadius = Mathf.Max(r.FieldRadius + 1f, despawnRadius);
            r.MinSpawnDistance = Mathf.Clamp(minSpawnDistance, 0f, r.FieldRadius - 1f);

            r.MinRadius = Mathf.Max(0.1f, minRadius);
            r.MaxRadius = Mathf.Max(r.MinRadius, maxRadius);

            r.InitialFillMultiplier = Mathf.Max(0.1f, initialFillMultiplier);
            // No hace falta llamar nada más: fieldUC.UpdateField() se encarga de reponer / limpiar con el tiempo.
        }

        [ContextMenu("Refill Initial Field (Heavy)")]
        public void RefillInitialField()
        {
            if (installer == null || installer.fieldUC == null || installer.gameState == null) return;

            // Opción “fuerte”: vaciar y repoblar según los valores actuales
            installer.gameState.Asteroids.Clear();
            ApplyNow(); // asegurarnos de que las reglas están al día
            installer.fieldUC.InitializeField(installer.gameState);
        }
    }
}
