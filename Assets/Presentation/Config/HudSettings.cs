using UnityEngine;

namespace VectorArcade.Presentation.Config
{
    [CreateAssetMenu(menuName = "VectorArcade/HUD Settings")]
    public sealed class HudSettings : ScriptableObject
    {
        [Min(0.1f)] public float CrosshairDistance = 2.5f;
        [Min(0.001f)] public float CrosshairSize = 0.05f; // longitud de media pata de la cruz
    }
}
