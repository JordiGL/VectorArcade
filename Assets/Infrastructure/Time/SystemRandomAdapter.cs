// Assets/Infrastructure/Time/SystemRandomAdapter.cs
using UnityEngine;
using VectorArcade.Application.Ports;

namespace VectorArcade.Infrastructure.Time
{
    public sealed class SystemRandomAdapter : MonoBehaviour, IRandomProvider
    {
        System.Random _rng;
        void Awake()
        {
            _rng = new System.Random(); // seed auto
        }
        public float NextFloat(float min, float max)
        {
            var t = (float)_rng.NextDouble();
            return min + (max - min) * t;
        }
    }
}
