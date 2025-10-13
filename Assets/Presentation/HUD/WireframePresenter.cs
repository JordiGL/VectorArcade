// Assets/Presentation/HUD/WireframePresenter.cs
using System;
using UnityEngine;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Core;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Presentation.HUD
{
    public static class WireframePresenter
    {
        // ───────────────── Retícula (siempre delante de la cámara)
        public static void DrawCrosshair(ILineRendererPort lines, Camera cam)
        {
            const float dist = 2.5f;  // distancia desde la cámara
            const float size = 0.12f; // mitad de longitud de cada trazo

            var t = cam.transform;
            Vector3 c = t.position + t.forward * dist;
            Vector3 r = t.right * size;
            Vector3 u = t.up * size;

            lines.AddLine((c - r).x, (c - r).y, (c - r).z, (c + r).x, (c + r).y, (c + r).z);
            lines.AddLine((c - u).x, (c - u).y, (c - u).z, (c + u).x, (c + u).y, (c + u).z);
        }

        // ───────────────── Asteroide: círculo segmentado en el plano XY
        public static void DrawAsteroid(ILineRendererPort lines, Asteroid a, int segments = 16)
        {
            float r = a.Radius;
            float z = a.Position.z;
            float cx = a.Position.x;
            float cy = a.Position.y;

            double step = (Math.PI * 2.0) / segments;
            float px = cx + r, py = cy;

            for (int i = 1; i <= segments; i++)
            {
                double t = step * i;
                float x = cx + r * (float)Math.Cos(t);
                float y = cy + r * (float)Math.Sin(t);
                lines.AddLine(px, py, z, x, y, z);
                px = x; py = y;
            }
        }

        // ───────────────── Bala: pequeño trazo en dirección Z local de la bala
        public static void DrawBullet(ILineRendererPort lines, Bullet b)
        {
            // Dibuja la bala a lo largo de su dirección de movimiento (velocity)
            const float len = 2.0f;

            // Normaliza la velocidad
            float vx = b.Velocity.x, vy = b.Velocity.y, vz = b.Velocity.z;
            float mag = Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
            if (mag < 1e-5f) mag = 1f; // evita división por cero

            float dx = vx / mag, dy = vy / mag, dz = vz / mag;

            float hx = dx * (len * 0.5f);
            float hy = dy * (len * 0.5f);
            float hz = dz * (len * 0.5f);

            float x = b.Position.x, y = b.Position.y, z = b.Position.z;

            lines.AddLine(x - hx, y - hy, z - hz, x + hx, y + hy, z + hz);
        }

        public static void DrawMissile(ILineRendererPort lines, Missile m)
        {
            // triángulo apuntando en dirección de su velocidad
            var pos = new UnityEngine.Vector3(m.Position.x, m.Position.y, m.Position.z);
            var v = new UnityEngine.Vector3(m.Velocity.x, m.Velocity.y, m.Velocity.z);
            if (v.sqrMagnitude < 1e-6f) v = UnityEngine.Vector3.forward;
            var dir = v.normalized;
            var right = UnityEngine.Vector3.Cross(UnityEngine.Vector3.up, dir).normalized;
            var up = UnityEngine.Vector3.Cross(dir, right);

            float len = 2.0f;
            float half = 0.6f;

            var tip = pos + dir * len;
            var a = pos - dir * 0.5f + right * half;
            var b = pos - dir * 0.5f - right * half;

            lines.AddLine(tip.x, tip.y, tip.z, a.x, a.y, a.z);
            lines.AddLine(tip.x, tip.y, tip.z, b.x, b.y, b.z);
            lines.AddLine(a.x, a.y, a.z, b.x, b.y, b.z);
        }

        public static void DrawItem(ILineRendererPort lines, Item it, Camera cam)
        {
            float s = 1.2f;
            var t = cam.transform;

            // base del rombo orientado al plano de cámara
            var pos = new UnityEngine.Vector3(it.Position.x, it.Position.y, it.Position.z);
            var right = t.right * s;
            var up = t.up * s;

            var a = pos - right * 0.5f;
            var b = pos + up * 0.5f;
            var c = pos + right * 0.5f;
            var d = pos - up * 0.5f;

            lines.AddLine(a.x, a.y, a.z, b.x, b.y, b.z);
            lines.AddLine(b.x, b.y, b.z, c.x, c.y, c.z);
            lines.AddLine(c.x, c.y, c.z, d.x, d.y, d.z);
            lines.AddLine(d.x, d.y, d.z, a.x, a.y, a.z);
        }

        // ───────────────── Dibuja todo
        public static void DrawAll(ILineRendererPort lines, GameState state, Camera cam)
        {
            DrawCrosshair(lines, cam);

            for (int i = 0; i < state.Asteroids.Count; i++)
                DrawAsteroid(lines, state.Asteroids[i], 18);

            for (int i = 0; i < state.Bullets.Count; i++)
                DrawBullet(lines, state.Bullets[i]);

            for (int i = 0; i < state.Missiles.Count; i++)
                DrawMissile(lines, state.Missiles[i]);

            for (int i = 0; i < state.Items.Count; i++)
                DrawItem(lines, state.Items[i], cam);
        }
    }
}
