// Assets/Presentation/Debug/WeaponRulesLiveTuner.cs
using UnityEngine;
using VectorArcade.Presentation.Bootstrap;

namespace VectorArcade.Presentation.DebugTools
{
    public sealed class WeaponRulesLiveTuner : MonoBehaviour
    {
        [Header("Scene wiring")]
        public GameInstaller installer;

        [Header("Live apply")]
        public bool applyContinuously = true;

        [Header("Weapon")]
        [Min(0.1f)] public float bulletSpeed = 100f;
        [Min(0.05f)] public float bulletLife  = 4.0f;
        [Min(0.1f)] public float fireRatePerSecond = 5.5f;
        [Min(0f)]   public float muzzleOffset = 1.5f;

        void Awake()
        {
            if (installer == null) return;
            var w = installer.weaponRules;
            bulletSpeed       = w.BulletSpeed;
            bulletLife        = w.BulletLife;
            fireRatePerSecond = w.FireRatePerSecond;
            muzzleOffset      = w.MuzzleOffset;
        }

        void Update()
        {
            if (!UnityEngine.Application.isPlaying || installer == null) return;
            if (applyContinuously) ApplyNow();
        }

        [ContextMenu("Apply Now")]
        public void ApplyNow()
        {
            if (installer == null) return;
            var w = installer.weaponRules;

            w.BulletSpeed       = Mathf.Max(0.1f, bulletSpeed);
            w.BulletLife        = Mathf.Max(0.05f, bulletLife);
            w.FireRatePerSecond = Mathf.Max(0.1f, fireRatePerSecond);
            w.MuzzleOffset      = Mathf.Max(0f, muzzleOffset);
        }
    }
}
