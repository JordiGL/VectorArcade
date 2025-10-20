using System.Collections.Generic;
using UnityEngine;
using VectorArcade.Application.Ports;

namespace VectorArcade.Infrastructure.Rendering
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class LineMeshBatchRenderer : MonoBehaviour, ILineRendererPort, IColorLineRendererPort
    {
        [SerializeField] Material lineMaterial;
        [SerializeField] bool interpretWorldSpace = true;

        Mesh _mesh;
        readonly List<Vector3> _verts = new(8192);
        readonly List<int> _indices = new(16384);
        readonly List<Color32> _colors = new(8192); // ← colores por vértice

        void Awake()
        {
            _mesh = new Mesh { name = "LineBatchMesh" };
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _mesh.MarkDynamic();

            GetComponent<MeshFilter>().sharedMesh = _mesh;
            var mr = GetComponent<MeshRenderer>();
            if (lineMaterial != null) mr.sharedMaterial = lineMaterial;
        }

        public void BeginFrame()
        {
            _verts.Clear();
            _indices.Clear();
            _colors.Clear();
        }

        // Implementación del puerto clásico (sin color) → blanco opaco
        public void AddLine(float ax, float ay, float az, float bx, float by, float bz)
            => AddLine(ax, ay, az, bx, by, bz, new Color32(255, 255, 255, 255));

        // Implementación del puerto con color
        public void AddLine(float ax, float ay, float az, float bx, float by, float bz, Rgba32 color)
            => AddLine(ax, ay, az, bx, by, bz, new Color32(color.r, color.g, color.b, color.a));

        // Interno que realmente añade con Color32 Unity
        void AddLine(float ax, float ay, float az, float bx, float by, float bz, Color32 color)
        {
            int i = _verts.Count;

            if (interpretWorldSpace)
            {
                var a = transform.InverseTransformPoint(new Vector3(ax, ay, az));
                var b = transform.InverseTransformPoint(new Vector3(bx, by, bz));
                _verts.Add(a);
                _verts.Add(b);
            }
            else
            {
                _verts.Add(new Vector3(ax, ay, az));
                _verts.Add(new Vector3(bx, by, bz));
            }

            _indices.Add(i);
            _indices.Add(i + 1);

            _colors.Add(color);
            _colors.Add(color);
        }

        public void EndFrame()
        {
            _mesh.Clear(false);
            _mesh.SetVertices(_verts);
            _mesh.SetIndices(_indices, MeshTopology.Lines, 0, false);
            _mesh.SetColors(_colors);
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(10000f, 10000f, 10000f));
        }
    }
}
