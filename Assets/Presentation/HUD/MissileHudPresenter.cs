// Assets/Presentation/HUD/MissileHudPresenter.cs
using UnityEngine;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Services;

namespace VectorArcade.Presentation.HUD
{
    // Separate HUD presenter for missiles to avoid touching non-UTF8 source files
    public static class MissileHudPresenter
    {
        // 7-segment font (digits only)
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

        private static void DrawDigit(ILineRendererPort lines, char ch, Vector3 origin, Vector3 right, Vector3 up, float size)
        {
            int mask = kDigitMasks[ch - '0'];
            for (int s = 0; s < 7; s++)
            {
                if ((mask & (1 << s)) == 0) continue;
                var a = kSegments[s].Item1;
                var b = kSegments[s].Item2;
                var A = origin + right * (a.x * size) + up * (a.y * size);
                var B = origin + right * (b.x * size) + up * (b.y * size);
                lines.AddLine(A.x, A.y, A.z, B.x, B.y, B.z);
            }
        }

        private static void DrawTextRightAligned(ILineRendererPort lines, Camera cam, string text,
                                                 float dist, float size, float spacing,
                                                 float vx, float vy)
        {
            var t = cam.transform;
            float step = size * (1f + spacing);
            // Compute the start pen so that the last character ends at vx
            Vector3 end = cam.ViewportToWorldPoint(new Vector3(vx, vy, dist));
            Vector3 pen = end - t.right * (step * (text.Length - 1));
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= '0' && c <= '9') DrawDigit(lines, c, pen, t.right, t.up, size);
                pen += t.right * step;
            }
        }

        private static void DrawMissileIcon(ILineRendererPort lines, Vector3 origin, Vector3 right, Vector3 up, float size)
        {
            // Simple rocket shape facing right
            Vector3 tip = origin + right * (0.95f * size) + up * (0.50f * size);
            Vector3 tailTop = origin + right * (0.20f * size) + up * (0.80f * size);
            Vector3 tailBottom = origin + right * (0.20f * size) + up * (0.20f * size);
            Vector3 finTop = origin + right * (0.35f * size) + up * (0.95f * size);
            Vector3 finBottom = origin + right * (0.35f * size) + up * (0.05f * size);

            // Body
            lines.AddLine(tip.x, tip.y, tip.z, tailTop.x, tailTop.y, tailTop.z);
            lines.AddLine(tip.x, tip.y, tip.z, tailBottom.x, tailBottom.y, tailBottom.z);
            lines.AddLine(tailTop.x, tailTop.y, tailTop.z, tailBottom.x, tailBottom.y, tailBottom.z);

            // Fins
            lines.AddLine(tailTop.x, tailTop.y, tailTop.z, finTop.x, finTop.y, finTop.z);
            lines.AddLine(tailBottom.x, tailBottom.y, tailBottom.z, finBottom.x, finBottom.y, finBottom.z);
        }

        public static void DrawMissiles(ILineRendererPort lines, GameState state, Camera cam)
        {
            var t = cam.transform;
            const float dist = 2.5f;
            const float icon = 0.16f;      // icon size
            const float text = 0.12f;      // digit size
            const float spacing = 0.20f;   // digit spacing

            // Icon at right bottom
            Vector3 msOrigin = cam.ViewportToWorldPoint(new Vector3(0.94f, 0.10f, dist));
            DrawMissileIcon(lines, msOrigin, t.right, t.up, icon);

            // Count to the left of the icon (right-aligned)
            string count = Mathf.Max(0, state.Player.MissilesLeft).ToString();
            DrawTextRightAligned(lines, cam, count, dist, text, spacing, 0.88f, 0.10f);
        }
    }
}

