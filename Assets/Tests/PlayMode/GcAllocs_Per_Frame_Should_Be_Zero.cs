using System.Collections;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TestTools;
using VectorArcade.Infrastructure.Rendering;

/// <summary>
/// Fitness test de rendimiento para el renderizador por líneas (<see cref="LineMeshBatchRenderer"/>).
/// 
/// Mide las asignaciones de memoria del GC por frame usando un enfoque de
/// <b>delta vs baseline</b>:
/// - Fase A (baseline): ejecuta el bucle del runner de UnityTest sin el renderer para
///   capturar el overhead inevitable de la propia infraestructura de test (corrutinas, yields).
/// - Fase B (medición): repite la toma con el renderer activo y un trabajo mínimo.
/// - Afirmación: el incremento (delta) respecto al baseline debe ser ≤ una pequeña tolerancia.
/// 
/// Este test no intenta demostrar que el GC sea estrictamente 0 B en cada frame (el runner
/// rara vez lo permite), sino que <b>el renderer no introduce asignaciones adicionales</b>.
/// </summary>
[Category("Fitness/Performance")]
public class GcAllocs_Per_Frame_Should_Be_Zero
{
    /// <summary>
    /// Ejecuta dos mediciones consecutivas de "GC Allocated In Frame":
    /// 1) <b>Baseline</b> sin renderer (calienta y mide 120 frames).
    /// 2) <b>Medición</b> con <see cref="LineMeshBatchRenderer"/> enviando 2 segmentos por frame.
    /// 
    /// Luego compara ambos picos (max) y verifica que el delta ≤ 1 KiB, margen que
    /// cubre pequeñas variaciones del scheduler o del propio Test Runner.
    /// </summary>
    [UnityTest]
    public IEnumerator Zero_Allocations_Delta_Vs_Baseline()
    {
        // --- FASE A: Baseline (sin renderer) -----------------------------
        var baseRec = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        long baseMax = 0;

        // Calentamiento
        for (int i = 0; i < 30; i++) yield return null;

        // Medición baseline (solo el overhead del runner)
        for (int i = 0; i < 120; i++)
        {
            yield return null;
            if (baseRec.LastValue > baseMax) baseMax = baseRec.LastValue;
        }
        baseRec.Dispose();

        // --- FASE B: Con el renderer ------------------------------------
        var goRenderer = new GameObject("RendererRoot");
        goRenderer.AddComponent<MeshFilter>();
        goRenderer.AddComponent<MeshRenderer>();
        var renderer = goRenderer.AddComponent<LineMeshBatchRenderer>();

        // Trabajo mínimo y estable por frame: 2 segmentos (4 vértices)
        void SubmitCrosshair()
        {
            renderer.BeginFrame();
            renderer.AddLine(-0.2f, 0f, 4f, 0.2f, 0f, 4f);
            renderer.AddLine(0f, -0.2f, 4f, 0f, 0.2f, 4f);
            renderer.EndFrame();
        }

        // Calentamiento con renderer
        for (int i = 0; i < 30; i++) { SubmitCrosshair(); yield return null; }

        var measRec = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        long measMax = 0;

        for (int i = 0; i < 120; i++)
        {
            SubmitCrosshair();
            yield return null;
            if (measRec.LastValue > measMax) measMax = measRec.LastValue;
        }
        measRec.Dispose();

        // --- ASSERT: el renderer no añade GC sobre el baseline -----------
        long delta = measMax - baseMax;

        // Tolerancia pequeña por jitter de frames / runner
        const long toleranceBytes = 1024; // 1 KiB

        Assert.LessOrEqual(
            delta, toleranceBytes,
            $"Tu renderer añade GC sobre el baseline del runner. baseline={baseMax} B, medido={measMax} B, delta={delta} B"
        );
    }
}
