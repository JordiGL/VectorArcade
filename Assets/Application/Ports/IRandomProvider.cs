// Assets/Application/Ports/IRandomProvider.cs
namespace VectorArcade.Application.Ports
{
    public interface IRandomProvider
    {
        float NextFloat(float min, float max);
    }
}
