using System.DoubleNumerics;

namespace raytracer;

internal class Lambertian : IMaterial
{
    public ITexture albedo;

    public Lambertian(ITexture albedo)
    {
        this.albedo = albedo;
    }

    public Lambertian(Vector3 albedo):this(new SolidColor(albedo))
    {
    }

    public bool Scatter(in Ray rayIn, in HitInfo hit, out Ray rayout)
    {
        var scatterDir = hit.Normal + Util.RandomInUnitSphere();

        if (scatterDir.NearZero())
            scatterDir = hit.Normal;

        scatterDir = scatterDir.Normalized();

        rayout =  new(hit.Position, scatterDir);

        return true;
    }

    Vector3 IMaterial.Color(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn)
    {
        return colorIn * albedo.Sample(hitInfo.uv);
    }
}
