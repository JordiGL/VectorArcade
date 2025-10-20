using UnityEngine;

namespace VectorArcade.Presentation.Config
{
    [CreateAssetMenu(menuName = "VectorArcade/HUD Settings")]
    public sealed class HudSettings : ScriptableObject
    {
        [Header("Crosshair Settings")]
        [Min(0.1f)] public float CrosshairDistance = 2.5f;
        [Min(0.001f)] public float CrosshairSize = 0.05f;

        [Header("Crosshair Color")]
        public CrosshairColor Color = CrosshairColor.White;
    }

    public enum CrosshairColor
    {
        White,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta
    }
}
