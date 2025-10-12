// Assets/Presentation/HUD/ScoreHud.cs
using UnityEngine;
using VectorArcade.Presentation.Bootstrap;

namespace VectorArcade.Presentation.HUD
{
    public sealed class ScoreHud : MonoBehaviour
    {
        public GameInstaller installer;
        public bool showFps = false;
        public bool showCounts = false;

        GUIStyle _style;
        float _fpsAccum;
        int _fpsFrames;
        float _fps;

        void Awake()
        {
            _style = new GUIStyle
            {
                fontSize = 18,
                normal = { textColor = Color.white }
            };
        }

        void Update()
        {
            // FPS suavizado (opcional)
            _fpsAccum += UnityEngine.Time.unscaledDeltaTime;
            _fpsFrames++;
            if (_fpsAccum >= 0.5f)
            {
                _fps = _fpsFrames / _fpsAccum;
                _fpsAccum = 0f; _fpsFrames = 0;
            }
        }

        void OnGUI()
        {
            if (installer == null || installer.gameState == null) return;

            float pad = 10f;
            var s = installer.gameState.Score;
            string line = $"SCORE {s}";

            if (showFps) line += $"   FPS {(int)_fps}";
            if (showCounts) line += $"   AST {installer.gameState.Asteroids.Count}  BLT {installer.gameState.Bullets.Count}";

            GUI.Label(new Rect(pad, pad, 600, 30), line, _style);
        }
    }
}
