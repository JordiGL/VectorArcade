namespace VectorArcade.Application.Ports
{
    /// <summary>
    /// Puerto opcional: permite enviar color por línea (RGBA 8-bit).
    /// El adaptador de rendering puede implementar este puerto además de ILineRendererPort.
    /// </summary>
    public interface IColorLineRendererPort : ILineRendererPort
    {
        void AddLine(float ax, float ay, float az, float bx, float by, float bz, Rgba32 color);
    }

    /// <summary>Color empaquetado RGBA en 8 bits por canal (sin depender de UnityEngine).</summary>
    public readonly struct Rgba32
    {
        public readonly byte r, g, b, a;
        public Rgba32(byte r, byte g, byte b, byte a = 255) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Rgba32 White(byte a = 255) => new Rgba32(255, 255, 255, a);
    }
}
