// Assets/Infrastructure/Input/KeyboardMouseInputAdapter.cs
using UnityEngine;
using VectorArcade.Application.Ports;

namespace VectorArcade.Infrastructure.Input
{
    public sealed class KeyboardMouseInputAdapter : MonoBehaviour, IInputProvider
    {
        [Header("Mouse look")]
        public float mouseSensitivity = 1.0f;

        public float MouseDeltaX { get; private set; }
        public float MouseDeltaY { get; private set; }

        public bool FirePrimary { get; private set; } // left / space
        public bool FireSecondary { get; private set; } // right

        void Update()
        {
            MouseDeltaX = UnityEngine.Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            MouseDeltaY = UnityEngine.Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            // Primary: botón izquierdo o Space
            bool leftClick = UnityEngine.Input.GetMouseButton(0);
            bool space = UnityEngine.Input.GetKey(KeyCode.Space);
            FirePrimary = leftClick || space;

            // Secondary: botón derecho
            FireSecondary = UnityEngine.Input.GetMouseButton(1);
        }

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked; // opcional, útil para FPS
            Cursor.visible = false;
        }
        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
