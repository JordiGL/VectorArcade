// Assets/Application/Ports/IInputProvider.cs
namespace VectorArcade.Application.Ports
{
    public interface IInputProvider
    {
        float MouseDeltaX { get; }
        float MouseDeltaY { get; }

        // Disparo primario (láser): botón izq. o barra espaciadora
        bool FirePrimary { get; }

        // Disparo secundario (misil): botón derecho
        bool FireSecondary { get; }
    }
}