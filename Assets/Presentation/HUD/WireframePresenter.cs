using UnityEngine;
using VectorArcade.Application.Ports;
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

        // ───────────────── Asteroide: wireframe 3D con forma irregular + rotación
        public static void DrawAsteroid3D(ILineRendererPort lines, Asteroid a, float timeSinceStart)
        {
            WireAsteroidShapes.Ensure(8);

            var shape = WireAsteroidShapes.Get(a.ShapeIndex);
            var verts = shape.verts;
            var edges = shape.edges;

            var axis = new Vector3(a.SpinAxis.x, a.SpinAxis.y, a.SpinAxis.z);
            if (axis.sqrMagnitude < 1e-8f) axis = Vector3.up;
            axis.Normalize();

            float angle = a.SpinPhase + a.SpinSpeed * timeSinceStart;
            var rot = Quaternion.AngleAxis(angle, axis);

            float s = a.Radius;
            var center = new Vector3(a.Position.x, a.Position.y, a.Position.z);

            // Si el puerto soporta color, lo usamos para atenuar “interiores”
            IColorLineRendererPort colorPort = lines as IColorLineRendererPort;

            // Dirección aproximada de cámara (para un efecto sencillo y barato)
            Vector3 viewDir = (Camera.main != null) ? Camera.main.transform.forward : Vector3.forward;

            for (int i = 0; i < edges.Length; i++)
            {
                var e = edges[i];
                Vector3 p0 = center + rot * (verts[e.a] * s);
                Vector3 p1 = center + rot * (verts[e.b] * s);

                if (colorPort != null)
                {
                    // Heurística barata: usamos el punto medio y su vector desde el centro para estimar “frente/espalda”
                    Vector3 mid = (p0 + p1) * 0.5f;
                    Vector3 fromCenter = (mid - center).normalized;

                    // dot ≈ 1 → hacia cámara → más opaco; dot ≈ 0 → lateral → tenue
                    float dot = Mathf.Abs(Vector3.Dot(fromCenter, viewDir));
                    byte alpha = (byte)Mathf.Clamp(Mathf.Lerp(40f, 255f, dot), 0f, 255f);

                    var col = new Rgba32(255, 255, 255, alpha);
                    colorPort.AddLine(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z, col);
                }
                else
                {
                    // Fallback: sin color → mismo grosor/alpha que antes
                    lines.AddLine(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
                }
            }
        }

        // ───────────────── Bala: pequeño trazo en dirección Z local de la bala
        public static void DrawBullet(ILineRendererPort lines, Bullet b)
        {
            const float len = 2.0f;

            float vx = b.Velocity.x, vy = b.Velocity.y, vz = b.Velocity.z;
            float mag = Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
            if (mag < 1e-5f) mag = 1f;

            float dx = vx / mag, dy = vy / mag, dz = vz / mag;

            float hx = dx * (len * 0.5f);
            float hy = dy * (len * 0.5f);
            float hz = dz * (len * 0.5f);

            float x = b.Position.x, y = b.Position.y, z = b.Position.z;

            lines.AddLine(x - hx, y - hy, z - hz, x + hx, y + hy, z + hz);
        }

        public static void DrawMissile(ILineRendererPort lines, Missile m)
        {
            var pos = new Vector3(m.Position.x, m.Position.y, m.Position.z);
            var v = new Vector3(m.Velocity.x, m.Velocity.y, m.Velocity.z);
            if (v.sqrMagnitude < 1e-6f) v = Vector3.forward;
            var dir = v.normalized;
            var right = Vector3.Cross(Vector3.up, dir).normalized;
            var up = Vector3.Cross(dir, right);

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

            var pos = new Vector3(it.Position.x, it.Position.y, it.Position.z);
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

            // Asteroides wireframe 3D con rotación temporal
            float t = state.TimeSinceStart;
            for (int i = 0; i < state.Asteroids.Count; i++)
                DrawAsteroid3D(lines, state.Asteroids[i], t);

            for (int i = 0; i < state.Bullets.Count; i++)
                DrawBullet(lines, state.Bullets[i]);

            for (int i = 0; i < state.Missiles.Count; i++)
                DrawMissile(lines, state.Missiles[i]);

            for (int i = 0; i < state.Items.Count; i++)
                DrawItem(lines, state.Items[i], cam);
        }
    }
}
