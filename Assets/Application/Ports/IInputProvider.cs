// Assets/Application/Ports/IInputProvider.cs
namespace VectorArcade.Application.Ports
{
    public interface IInputProvider
    {
        bool FirePressed { get; }

        // Delta de ratón por frame (ejes clásicos)
        float MouseDeltaX { get; }
        float MouseDeltaY { get; }
    }
}
