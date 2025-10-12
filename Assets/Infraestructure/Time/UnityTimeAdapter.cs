// Assets/Infrastructure/Time/UnityTimeAdapter.cs
using UnityEngine;
using VectorArcade.Application.Ports;
// Alias para evitar colisión con el namespace VectorArcade.Infrastructure.Time
using UTime = UnityEngine.Time;

namespace VectorArcade.Infrastructure.Time
{
    public sealed class UnityTimeAdapter : MonoBehaviour, ITimeProvider
    {
        public float DeltaTime => UTime.deltaTime;
    }
}
