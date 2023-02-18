using System.DoubleNumerics;

namespace raytracer;
internal interface ITexture<T>
{
    T Sample(in Vector2 uv, bool raw=false);
}

internal interface ITexture3:ITexture<Vector3>
{
}
internal interface ITexture1 : ITexture<double>
{
    public Vector3 Normal(in Vector2 uv)
    {
        double middle = Sample(uv, raw: true);
        double dfdx = (Sample(uv + Vector2.UnitX, raw:true) - middle);
        double dfdy = (Sample(uv + Vector2.UnitY, raw: true) - middle);
        double mag = Math.Sqrt(dfdx * dfdx + dfdy * dfdy + 1);
        return new Vector3(dfdx/mag, dfdy/mag, 1/mag);
    }
}

internal class SolidColor : ITexture3
{
    public readonly Vector3 color;

    public SolidColor(Vector3 color)
    {
        this.color = color;
    }

    public Vector3 Sample(in Vector2 uv, bool raw) => color;
}