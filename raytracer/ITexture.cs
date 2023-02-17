using System.DoubleNumerics;

namespace raytracer;

internal interface ITexture
{
    Vector3 Sample(in Vector2 uv);
}

internal class SolidColor : ITexture
{
    public readonly Vector3 color;

    public SolidColor(Vector3 color)
    {
        this.color = color;
    }

    public Vector3 Sample(in Vector2 uv) => color;
}