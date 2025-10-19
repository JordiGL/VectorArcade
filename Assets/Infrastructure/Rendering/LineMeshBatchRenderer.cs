using System.Collections.Generic;
using UnityEngine;
using VectorArcade.Application.Ports;

namespace VectorArcade.Infrastructure.Rendering
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class LineMeshBatchRenderer : MonoBehaviour, ILineRendererPort
    {
        [SerializeField] Material lineMaterial;

        // Nuevo: interpreta las posiciones recibidas como coordenadas de MUNDO.
        [SerializeField] bool interpretWorldSpace = true;

        Mesh _mesh;
        readonly List<Vector3> _verts = new(8192);
        readonly List<int> _indices = new(16384);

        void Awake()
        {
            _mesh = new Mesh { name = "LineBatchMesh" };
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            GetComponent<MeshFilter>().sharedMesh = _mesh;

            var mr = GetComponent<MeshRenderer>();
            if (lineMaterial != null) mr.sharedMaterial = lineMaterial;
        }

        public void BeginFrame()
        {
            _verts.Clear();
            _indices.Clear();
        }

        public void AddLine(float ax, float ay, float az, float bx, float by, float bz)
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
        }

        public void EndFrame()
        {
            _mesh.Clear(false); // no borra el layout → menos GC/CPU
            _mesh.SetVertices(_verts);
            _mesh.SetIndices(_indices, MeshTopology.Lines, 0, false);
            // Evita calcular bounds cada frame; usa un AABB amplio y estable
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(10000f, 10000f, 10000f));
        }

    }
}
