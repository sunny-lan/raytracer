using System.DoubleNumerics;

namespace raytracer;

internal class Metal : IMaterial
{
    public Vector3 Color;

    public Metal(Vector3 color)
    {
        Color = color;
    }

    public bool Scatter(Ray rayIn, HitInfo hit, out Ray rayout)
    {
        var scatterDir = rayIn.Direction - 2 * hit.Normal * Vector3.Dot(rayIn.Direction, hit.Normal);

        rayout = new(hit.Position, scatterDir);
        return true;
    }

    Vector3 IMaterial.Color(Ray rayIn, HitInfo hitInfo, Vector3 colorIn)
    {
        return colorIn * Color;
    }
}

internal class Light : Lambertian, IMaterial
{
    public Light(Vector3 color):base(color)
    {
    }

    bool IMaterial.Scatter(Ray rayIn, HitInfo hitInfo, out Ray rayout)
    {
        return base.Scatter(rayIn,hitInfo,out rayout); 
    }

    Vector3 IMaterial.Color(Ray rayIn, HitInfo hitInfo, Vector3 colorIn)
    {
        return colorIn + Color;
    }
}