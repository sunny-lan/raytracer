using System.DoubleNumerics;

namespace raytracer;

internal class Lambertian : IMaterial
{
    public ITexture3 albedo;

    public Lambertian(ITexture3 albedo)
    {
        this.albedo = albedo;
    }

    public Lambertian(Vector3 albedo):this(new SolidColor(albedo))
    {
    }

    public bool Scatter(in Ray rayIn, in HitInfo hit, out Ray rayout)
    {
        var scatterDir = hit.Normal + Util.RandomInUnitSphere().Normalized();

        if (scatterDir.NearZero())
            scatterDir = hit.Normal;

        scatterDir = scatterDir.Normalized();

        rayout =  new(hit.Position, scatterDir);

        return true;
    }

    Vector3 IMaterial.Reflect(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn, in Ray raySrc)
    {
        // Strength of reflectance based on cosine of angle to normal
        double cosine = Vector3.Dot(hitInfo.Normal, raySrc.Direction) / hitInfo.Normal.Length() / raySrc.Direction.Length();
        
        // Ray within sphere not reflected
        if(cosine < 0) return Vector3.Zero;
        return cosine / Math.PI * colorIn * albedo.Sample(hitInfo.uv);
    }
}
