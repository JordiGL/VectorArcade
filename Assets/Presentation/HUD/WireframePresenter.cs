using System;
using UnityEngine;
using VectorArcade.Application.Ports;
using VectorArcade.Domain.Entities;
using VectorArcade.Domain.Services;

namespace VectorArcade.Presentation.HUD
{
    public static class WireframePresenter
    {

        static Rgba32 ToRgba32(Color c)
        {
            byte r = (byte)Mathf.Clamp(Mathf.RoundToInt(c.r * 255f), 0, 255);
            byte g = (byte)Mathf.Clamp(Mathf.RoundToInt(c.g * 255f), 0, 255);
            byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(c.b * 255f), 0, 255);
            byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(c.a * 255f), 0, 255);
            return new Rgba32(r, g, b, a);
        }

        static void AddLineSmart(ILineRendererPort lines, Vector3 a, Vector3 b, Color color)
        {
            // Si el renderer soporta color, lo usamos
            if (lines is IColorLineRendererPort colorPort)
            {
                colorPort.AddLine(a.x, a.y, a.z, b.x, b.y, b.z, ToRgba32(color));
            }
            else
            {
                // Fallback: blanco sin color
                lines.AddLine(a.x, a.y, a.z, b.x, b.y, b.z);
            }
        }

        // Núcleo común: asteroide/debris con mismo estilo y sombreado
        static void DrawAsteroidWireCore(
            ILineRendererPort lines,
            int shapeIndex,
            Vector3 center,
            Vector3 axis,
            float radius,
            float spinSpeed,
            float spinPhase,
            float time,
            float alphaMul)
        {
            WireAsteroidShapes.Ensure(8);
            var shape = WireAsteroidShapes.Get(shapeIndex);
            var verts = shape.verts;
            var edges = shape.edges;

            if (axis.sqrMagnitude < 1e-8f) axis = Vector3.up;
            axis.Normalize();

            float angle = spinPhase + spinSpeed * time;
            var rot = Quaternion.AngleAxis(angle, axis);

            float s = Mathf.Max(0.001f, radius);

            IColorLineRendererPort colorPort = lines as IColorLineRendererPort;
            Vector3 viewDir = (Camera.main != null) ? Camera.main.transform.forward : Vector3.forward;

            for (int i = 0; i < edges.Length; i++)
            {
                var e = edges[i];
                Vector3 p0 = center + rot * (verts[e.a] * s);
                Vector3 p1 = center + rot * (verts[e.b] * s);

                if (colorPort != null)
                {
                    Vector3 mid = (p0 + p1) * 0.5f;
                    Vector3 fromCenter = (mid - center).normalized;
                    float dot = Mathf.Abs(Vector3.Dot(fromCenter, viewDir));
                    float a = Mathf.Clamp(Mathf.Lerp(40f, 255f, dot), 0f, 255f) * Mathf.Clamp01(alphaMul);
                    var col = new Rgba32(255, 255, 255, (byte)a);
                    colorPort.AddLine(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z, col);
                }
                else
                {
                    lines.AddLine(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
                }
            }
        }

        // ───────────────── Retícula (siempre delante de la cámara)
        public static void DrawCrosshair(ILineRendererPort lines, Camera cam, float dist, float size)
        {
            var t = cam.transform;
            Vector3 c = t.position + t.forward * dist;
            Vector3 r = t.right * size;
            Vector3 u = t.up * size;

            lines.AddLine((c - r).x, (c - r).y, (c - r).z, (c + r).x, (c + r).y, (c + r).z);
            lines.AddLine((c - u).x, (c - u).y, (c - u).z, (c + u).x, (c + u).y, (c + u).z);
        }

        public static void DrawCrosshair(ILineRendererPort lines, Camera cam, float dist, float size, Color color)
        {
            var t = cam.transform;
            Vector3 c = t.position + t.forward * dist;
            Vector3 r = t.right * size;
            Vector3 u = t.up * size;

            AddLineSmart(lines, c - r, c + r, color);
            AddLineSmart(lines, c - u, c + u, color);
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

        public static void DrawPlanet(ILineRendererPort lines, Planet p, int segments = 48)
        {
            // Esfera wireframe mejorada: múltiples paralelos y meridianos + anillo ecuatorial reforzado
            float r = p.Radius;
            var center = new Vector3(p.Position.x, p.Position.y, p.Position.z);

            IColorLineRendererPort colorPort = lines as IColorLineRendererPort;
            Vector3 viewDir = (Camera.main != null) ? Camera.main.transform.forward : Vector3.forward;

            void DrawCircleSmart(Vector3 c, Vector3 ex, Vector3 ey, byte alpha)
            {
                double step = (Math.PI * 2.0) / segments;
                Vector3 prev = c + ex * r;
                for (int i = 1; i <= segments; i++)
                {
                    double t = step * i;
                    Vector3 pt = c + (float)Math.Cos(t) * ex * r + (float)Math.Sin(t) * ey * r;
                    if (colorPort != null)
                        colorPort.AddLine(prev.x, prev.y, prev.z, pt.x, pt.y, pt.z, new Rgba32(255, 255, 255, alpha));
                    else
                        lines.AddLine(prev.x, prev.y, prev.z, pt.x, pt.y, pt.z);
                    prev = pt;
                }
            }

            // Anillo ecuatorial reforzado (triple pasada)
            DrawCircleSmart(center, Vector3.right, Vector3.forward, 220);
            DrawCircleSmart(center, Vector3.right * 0.98f, Vector3.forward * 0.98f, 140);
            DrawCircleSmart(center, Vector3.right * 1.02f, Vector3.forward * 1.02f, 140);

            // Paralelos (latitudes)
            int latRings = 6;
            for (int i = 1; i <= latRings; i++)
            {
                float t = i / (float)(latRings + 1);
                float phi = Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, t);
                float cphi = Mathf.Cos(phi);
                float sphi = Mathf.Sin(phi);
                Vector3 ex = Vector3.right * cphi;
                Vector3 ey = Vector3.forward * cphi;
                Vector3 c = center + Vector3.up * (sphi * r);
                float ndot = Mathf.Abs(Vector3.Dot(Vector3.up, viewDir));
                byte a = (byte)Mathf.Clamp(Mathf.Lerp(60f, 160f, 1f - ndot), 40f, 200f);
                DrawCircleSmart(c, ex, ey, a);
            }

            // Meridianos (longitudes)
            int lonRings = 10;
            for (int j = 0; j < lonRings; j++)
            {
                float ang = (Mathf.PI * 2f) * (j / (float)lonRings);
                Vector3 h = (Vector3.right * Mathf.Cos(ang) + Vector3.forward * Mathf.Sin(ang)).normalized;
                Vector3 ex = h;
                Vector3 ey = Vector3.up;
                Vector3 planeNormal = Vector3.Cross(ex, ey).normalized;
                float v = Mathf.Abs(Vector3.Dot(planeNormal, viewDir));
                byte a = (byte)Mathf.Clamp(Mathf.Lerp(70f, 180f, v), 50f, 200f);
                DrawCircleSmart(center, ex, ey, a);
            }
        }

        public static void DrawPlanetDebris(ILineRendererPort lines, PlanetDebris d, float time)
        {
            var center = new Vector3(d.Position.x, d.Position.y, d.Position.z);
            var axis = new Vector3(d.SpinAxis.x, d.SpinAxis.y, d.SpinAxis.z);
            int shape = Mathf.Abs(d.ShapeIndex) % WireAsteroidShapes.Count;
            DrawAsteroidWireCore(lines, shape, center, axis, Mathf.Max(0.4f, d.Radius), d.SpinSpeed, d.SpinPhase, time, 1f);
        }
        // ───────────────── Cometa: cabeza estilizada + cola con restos
        public static void DrawComet(ILineRendererPort lines, Comet c, float time)
        {
            var pos = new Vector3(c.Position.x, c.Position.y, c.Position.z);
            var vel = new Vector3(c.Velocity.x, c.Velocity.y, c.Velocity.z);
            if (vel.sqrMagnitude < 1e-6f) vel = Vector3.forward;
            var dir = vel.normalized;

            // Base ortonormal para orientar la forma
            var right = Vector3.Cross(Vector3.up, dir);
            if (right.sqrMagnitude < 1e-6f) right = Vector3.right;
            right.Normalize();
            var up = Vector3.Cross(dir, right).normalized;

            // Cabeza del cometa (punta + base)
            float head = Mathf.Max(1.2f, c.Radius * 2.0f);
            var tip = pos + dir * head;
            var baseCenter = pos - dir * (head * 0.35f);
            var bl = baseCenter - right * (head * 0.45f);
            var br = baseCenter + right * (head * 0.45f);
            var bu = baseCenter + up * (head * 0.35f);
            var bd = baseCenter - up * (head * 0.35f);

            // Contorno de la cabeza (rombo estilizado)
            AddLineSmart(lines, tip, br, new Color(1f, 1f, 1f, 0.95f));
            AddLineSmart(lines, tip, bl, new Color(1f, 1f, 1f, 0.95f));
            AddLineSmart(lines, br, bd, new Color(1f, 1f, 1f, 0.8f));
            AddLineSmart(lines, bd, bl, new Color(1f, 1f, 1f, 0.8f));
            AddLineSmart(lines, bl, bu, new Color(1f, 1f, 1f, 0.8f));
            AddLineSmart(lines, bu, br, new Color(1f, 1f, 1f, 0.8f));

            // Cola segmentada con pequeño jitter y alpha decreciente
            int segments = 7;
            float segLen = head * 0.9f;
            var cur = pos;
            for (int i = 0; i < segments; i++)
            {
                float t = (i + 1) / (float)segments;
                float alpha = Mathf.Lerp(0.85f, 0.12f, t);

                // Jitter pseudo-animado para una cola viva
                float ph = time * 3.0f + i * 1.7f;
                float amp = head * 0.12f * (1f - t);
                var jitter = right * (Mathf.Sin(ph) * amp) + up * (Mathf.Cos(ph * 0.9f) * amp);

                var next = cur - dir * segLen + jitter;
                AddLineSmart(lines, cur, next, new Color(1f, 1f, 1f, alpha));
                cur = next;
            }

            // Restos: pequeños trazos laterales cerca de la cola
            int debris = 6;
            float tailSpan = head * 6.0f;
            for (int k = 0; k < debris; k++)
            {
                float u = (k + 1f) / (debris + 1f);
                var basePt = pos - dir * (u * tailSpan);
                float ph = time * 2.2f + k * 0.77f;
                float off = head * 0.35f * (1f - u);
                var a = basePt + (right * Mathf.Sin(ph) + up * Mathf.Cos(ph)) * off;
                var b = a - dir * (head * 0.25f * (1f - u));
                AddLineSmart(lines, a, b, new Color(1f, 1f, 1f, 0.18f));
            }
        }

        // ───────────────── Dibuja todo
        public static void DrawAll(ILineRendererPort lines, GameState state, Camera cam, float crosshairDist, float crosshairSize, Color crosshairColor)
        {
            DrawCrosshair(lines, cam, crosshairDist, crosshairSize, crosshairColor);

            // Asteroides wireframe 3D con rotación temporal
            float t = state.TimeSinceStart;
            for (int i = 0; i < state.Asteroids.Count; i++)
                DrawAsteroid3D(lines, state.Asteroids[i], t);

            // Planets (esfera wireframe)
            for (int i = 0; i < state.Planets.Count; i++)
                DrawPlanet(lines, state.Planets[i], 24);

            for (int i = 0; i < state.Bullets.Count; i++)
                DrawBullet(lines, state.Bullets[i]);

            for (int i = 0; i < state.Missiles.Count; i++)
                DrawMissile(lines, state.Missiles[i]);

            for (int i = 0; i < state.Items.Count; i++)
                DrawItem(lines, state.Items[i], cam);

            for (int i = 0; i < state.PlanetDebris.Count; i++)
                DrawPlanetDebris(lines, state.PlanetDebris[i], t);

            for (int i = 0; i < state.Comets.Count; i++)
                DrawComet(lines, state.Comets[i], t);
        }
    }
}
