// Assets/Infrastructure/Input/KeyboardMouseInputAdapter.cs
using UnityEngine;
using VectorArcade.Application.Ports;
using UInput = UnityEngine.Input;

namespace VectorArcade.Infrastructure.Input
{
    public sealed class KeyboardMouseInputAdapter : MonoBehaviour, IInputProvider
    {
        [SerializeField] KeyCode fireKey = KeyCode.Space;

        public bool FirePressed => UInput.GetKey(fireKey) || UInput.GetMouseButton(0);
        public float MouseDeltaX => UInput.GetAxis("Mouse X");
        public float MouseDeltaY => UInput.GetAxis("Mouse Y");

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
