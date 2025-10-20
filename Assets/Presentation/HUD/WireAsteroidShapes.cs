using System;
using System.Collections.Generic;
using UnityEngine;

namespace VectorArcade.Presentation.HUD
{
    /// Genera y cachea plantillas wireframe de rocas (icosaedro con jitter radial).
    internal static class WireAsteroidShapes
    {
        internal sealed class Shape
        {
            public Vector3[] verts;
            public (int a, int b)[] edges;
        }

        static readonly List<Shape> _shapes = new();
        static (int a, int b)[] _icoEdges;
        static Vector3[] _icoVerts;

        public static int Count => _shapes.Count;

        public static void Ensure(int minCount, float jitter = 0.35f)
        {
            if (_icoVerts == null) BuildIcosahedron();
            if (_shapes.Count >= minCount) return;

            for (int i = _shapes.Count; i < minCount; i++)
            {
                var rnd = new System.Random(1337 + i);

                var vsrc = _icoVerts;
                var verts = new Vector3[vsrc.Length];
                for (int k = 0; k < vsrc.Length; k++)
                {
                    var v = vsrc[k].normalized;
                    float scale = 1f + (float)(rnd.NextDouble() * 2.0 - 1.0) * jitter;
                    verts[k] = v * scale;
                }

                var edges = _icoEdges;
                _shapes.Add(new Shape { verts = verts, edges = edges });
            }
        }

        public static Shape Get(int index)
        {
            if (_shapes.Count == 0) Ensure(8);
            int i = Mathf.Abs(index) % _shapes.Count;
            return _shapes[i];
        }

        static void BuildIcosahedron()
        {
            float t = (1f + Mathf.Sqrt(5f)) * 0.5f;
            var verts = new List<Vector3>
            {
                new(+1, +t,  0), new(-1, +t,  0), new(+1, -t,  0), new(-1, -t,  0),
                new( 0, +1, +t), new( 0, -1, +t), new( 0, +1, -t), new( 0, -1, -t),
                new(+t,  0, +1), new(-t,  0, +1), new(+t,  0, -1), new(-t,  0, -1),
            };
            for (int i = 0; i < verts.Count; i++) verts[i] = verts[i].normalized;

            var edges = new (int, int)[]
            {
                (0,1),(0,4),(0,6),(0,8),(0,10),
                (1,4),(1,6),(1,9),(1,11),
                (2,3),(2,5),(2,7),(2,8),(2,10),
                (3,5),(3,7),(3,9),(3,11),
                (4,5),(4,6),(4,8),(4,9),
                (5,8),(5,9),
                (6,10),(6,11),
                (7,10),(7,11),
                (8,10),
                (9,11)
            };

            _icoVerts = verts.ToArray();
            _icoEdges = edges;
        }
    }
}
