using VectorArcade.Application.Ports;

// Fakes comunes para todos los tests de UseCases
namespace VectorArcade.Tests.Fakes
{
    // Fake de tiempo (simula Time.deltaTime)
    public sealed class FakeTime : ITimeProvider
    {
        public float DeltaTime { get; set; }
        public float Time { get; set; }
    }

    // Fake de input (simula ratón y disparos)
    public sealed class FakeInput : IInputProvider
    {
        public float MouseDeltaX { get; set; }
        public float MouseDeltaY { get; set; }
        public bool FirePrimary { get; set; }
        public bool FireSecondary { get; set; }
    }
}
