// Assets/Application/Ports/ILineRendererPort.cs
namespace VectorArcade.Application.Ports
{
    public interface ILineRendererPort
    {
        void BeginFrame();
        void AddLine(float ax, float ay, float az, float bx, float by, float bz);
        void EndFrame();
    }
}
