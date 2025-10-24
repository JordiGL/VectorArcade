using System;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Services;

namespace VectorArcade.Application.UseCases
{
    /// Controla la cámara del jugador: yaw + pitch para apuntar.
    /// El movimiento sigue siendo recto en +Z mundial (no depende de la mirada).
    public sealed class PlayerControlUseCase
    {
        private readonly IInputProvider _input;
        private readonly ITimeProvider _time;

        const float MOUSE_SENS_X = 0.12f; // yaw
        const float MOUSE_SENS_Y = 0.12f; // pitch
        const float FORWARD_SPEED = 18f;  // u/s

        float yaw;   // grados
        float pitch; // grados

        static readonly Vec3 kMoveDir = new Vec3(0, 0, 1); // avance recto

        public PlayerControlUseCase(IInputProvider input, ITimeProvider time)
        {
            _input = input;
            _time = time;
        }

        public void Execute(GameState state)
        {
            // 1) Rotación a partir del ratón
            yaw += _input.MouseDeltaX * MOUSE_SENS_X;
            pitch -= _input.MouseDeltaY * MOUSE_SENS_Y;          // invertido como FPS
            pitch = Math.Clamp(pitch, -80f, 80f);               // evita mirar demasiado arriba/abajo

            // 2) Dirección de mirada (forward) con yaw + pitch
            float ry = yaw * (float)Math.PI / 180f;
            float rp = pitch * (float)Math.PI / 180f;
            float cy = (float)Math.Cos(ry);
            float sy = (float)Math.Sin(ry);
            float cp = (float)Math.Cos(rp);
            float sp = (float)Math.Sin(rp);

            // Convención: +Z hacia delante en mundo.
            var aimForward = new Vec3(sy * cp, sp, cy * cp);
            state.Player.Forward = aimForward;

            // 3) Avance constante en +Z mundial (independiente de la mirada)
            float dt = _time.DeltaTime;

            state.Player.Speed = FORWARD_SPEED;
            var vel = kMoveDir * FORWARD_SPEED;
            state.Player.Velocity = vel;
            state.Player.Position = state.Player.Position + vel * dt;
        }
    }
}
