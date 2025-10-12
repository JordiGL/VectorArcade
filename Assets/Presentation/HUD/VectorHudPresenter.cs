// Assets/Presentation/HUD/VectorHudPresenter.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Services;

namespace VectorArcade.Presentation.HUD
{
    /// Texto estilo Atari con segmentos, dibujado en el plano de la cámara.
    public static class VectorHudPresenter
    {
        // ───────── Fuente 7-segmentos ─────────
        private static readonly (Vector2, Vector2)[] kSegments = new (Vector2, Vector2)[]
        {
            (new Vector2(0.1f, 0.9f), new Vector2(0.9f, 0.9f)), // a
            (new Vector2(0.9f, 0.9f), new Vector2(0.9f, 0.5f)), // b
            (new Vector2(0.9f, 0.5f), new Vector2(0.9f, 0.1f)), // c
            (new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.1f)), // d
            (new Vector2(0.1f, 0.5f), new Vector2(0.1f, 0.1f)), // e
            (new Vector2(0.1f, 0.9f), new Vector2(0.1f, 0.5f)), // f
            (new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.5f)), // g
        };

        private static readonly int[] kDigitMasks = new int[]
        {
            /*0*/ (1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<4)|(1<<5),
            /*1*/ (1<<1)|(1<<2),
            /*2*/ (1<<0)|(1<<1)|(1<<6)|(1<<4)|(1<<3),
            /*3*/ (1<<0)|(1<<1)|(1<<6)|(1<<2)|(1<<3),
            /*4*/ (1<<5)|(1<<6)|(1<<1)|(1<<2),
            /*5*/ (1<<0)|(1<<5)|(1<<6)|(1<<2)|(1<<3),
            /*6*/ (1<<0)|(1<<5)|(1<<6)|(1<<4)|(1<<2)|(1<<3),
            /*7*/ (1<<0)|(1<<1)|(1<<2),
            /*8*/ (1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<4)|(1<<5)|(1<<6),
            /*9*/ (1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<5)|(1<<6),
        };

        private static readonly Dictionary<char, (int mask, (Vector2, Vector2)[] extra)> kLetters
            = new Dictionary<char, (int, (Vector2, Vector2)[])>
        {
            { 'S', ((1<<0)|(1<<5)|(1<<6)|(1<<2)|(1<<3), Array.Empty<(Vector2, Vector2)>()) },
            { 'C', ((1<<0)|(1<<5)|(1<<4)|(1<<3), Array.Empty<(Vector2, Vector2)>()) },
            { 'O', (kDigitMasks[0], Array.Empty<(Vector2, Vector2)>()) },
            { 'R', ((1<<0)|(1<<5)|(1<<1)|(1<<6)|(1<<4), new (Vector2, Vector2)[]{ (new(0.5f,0.5f), new(0.9f,0.1f)) }) },
            { 'E', ((1<<0)|(1<<5)|(1<<6)|(1<<4)|(1<<3), Array.Empty<(Vector2, Vector2)>()) },
            { ' ', (0, Array.Empty<(Vector2, Vector2)>()) },
            { 'F', ((1<<0)|(1<<5)|(1<<6)|(1<<4), Array.Empty<(Vector2, Vector2)>()) }, // para FPS
            { 'P', ((1<<0)|(1<<5)|(1<<1)|(1<<6)|(1<<4), Array.Empty<(Vector2, Vector2)>() ) }, // para FPS
        };

        private static void DrawChar(ILineRendererPort lines, char ch, Vector3 origin, Vector3 right, Vector3 up, float size)
        {
            void AddLocal(Vector2 a, Vector2 b)
            {
                var A = origin + right * (a.x * size) + up * (a.y * size);
                var B = origin + right * (b.x * size) + up * (b.y * size);
                lines.AddLine(A.x, A.y, A.z, B.x, B.y, B.z);
            }

            if (char.IsDigit(ch))
            {
                int mask = kDigitMasks[ch - '0'];
                for (int s = 0; s < 7; s++) if ((mask & (1 << s)) != 0) AddLocal(kSegments[s].Item1, kSegments[s].Item2);
                return;
            }

            char upc = char.ToUpperInvariant(ch);
            if (kLetters.TryGetValue(upc, out var entry))
            {
                int mask = entry.mask;
                for (int s = 0; s < 7; s++) if ((mask & (1 << s)) != 0) AddLocal(kSegments[s].Item1, kSegments[s].Item2);
                var extras = entry.extra;
                for (int i = 0; i < extras.Length; i++) AddLocal(extras[i].Item1, extras[i].Item2);
            }
        }

        // ───────── Helpers de texto en viewport ─────────
        static void DrawTextAtViewport(ILineRendererPort lines, Camera cam, string text,
                                       float dist, float size, float spacing,
                                       float vx, float vy)
        {
            var t = cam.transform;
            Vector3 origin = cam.ViewportToWorldPoint(new Vector3(vx, vy, dist));
            float step = size * (1f + spacing);
            Vector3 pen = origin;
            for (int i = 0; i < text.Length; i++)
            {
                DrawChar(lines, text[i], pen, t.right, t.up, size);
                pen += t.right * step;
            }
        }

        static void DrawTextAtViewportRightAligned(ILineRendererPort lines, Camera cam, string text,
                                                   float dist, float size, float spacing,
                                                   float vx, float vy)
        {
            var t = cam.transform;
            Vector3 end = cam.ViewportToWorldPoint(new Vector3(vx, vy, dist));
            float step = size * (1f + spacing);
            float total = text.Length * step;
            Vector3 start = end - t.right * total;
            Vector3 pen = start;
            for (int i = 0; i < text.Length; i++)
            {
                DrawChar(lines, text[i], pen, t.right, t.up, size);
                pen += t.right * step;
            }
        }

        // ───────── SCORE (top-left) ─────────
        public static void DrawScore(ILineRendererPort lines, GameState state, Camera cam)
        {
            string label = "SCORE " + state.Score.ToString();
            const float dist = 2.5f, size = 0.12f, spacing = 0.2f;
            DrawTextAtViewport(lines, cam, label, dist, size, spacing, 0.06f, 0.94f);
        }

        // ───────── FPS (top-right) ─────────
        static float _fpsAccum, _fpsFrames, _fps;
        const float _fpsInterval = 0.5f;

        public static void UpdateFps()
        {
            _fpsAccum += UnityEngine.Time.unscaledDeltaTime;
            _fpsFrames += 1f;
            if (_fpsAccum >= _fpsInterval)
            {
                _fps = _fpsFrames / _fpsAccum;
                _fpsAccum = 0f; _fpsFrames = 0f;
            }
        }

        public static void DrawFps(ILineRendererPort lines, Camera cam)
        {
            string label = "FPS " + ((int)_fps).ToString();
            const float dist = 2.5f, size = 0.12f, spacing = 0.2f;
            // margen 6% desde la derecha y 94% de altura
            DrawTextAtViewportRightAligned(lines, cam, label, dist, size, spacing, 0.94f, 0.94f);
        }
    }
}
