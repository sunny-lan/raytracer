using System.DoubleNumerics;

namespace raytracer;

internal class Metal : IMaterial
{
    public Vector3 Color;

    public Metal(Vector3 color)
    {
        Color = color;
    }

    public bool Scatter(in Ray rayIn, in HitInfo hit, out Ray rayout)
    {
        var scatterDir = rayIn.Direction - 2 * hit.Normal * Vector3.Dot(rayIn.Direction, hit.Normal);

        rayout = new(hit.Position, scatterDir);
        return true;
    }

    Vector3 IMaterial.Color(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn)
    {
        return colorIn * Color;
    }
}

internal class Light : Lambertian, IMaterial
{
    public Light(Vector3 color):base(color)
    {
    }

    bool IMaterial.Scatter(in Ray rayIn, in HitInfo hitInfo, out Ray rayout)
    {
        return base.Scatter(rayIn,hitInfo,out rayout); 
    }

    Vector3 IMaterial.Color(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn)
    {
        return colorIn + Color;
    }
}