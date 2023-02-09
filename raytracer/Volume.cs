using System.DoubleNumerics;

namespace raytracer;

internal class Volume : IHittable
{
    public IHittable boundary;
    public IMaterial material;
    public double density;

    public Volume(IHittable boundary, double density, IMaterial material)
    {
        this.boundary = boundary;
        this.material = material;
        this.density = density;
    }

    public bool Hit(Ray r, double tmin, double tmax, out HitInfo rec)
    {
        rec = new();

        if(!boundary.Hit(r, tmin, double.PositiveInfinity, out var hit))
        {
            return false;
        }

        double oldtmax = tmax;

        if (hit.FrontFace)
        {
            //entering
            tmin = hit.T;
            if(!boundary.Hit(r, tmin + 0.0001, double.PositiveInfinity, out var hit2))
            {
                return false; //this shouldn't happen?
            }

            tmax = hit2.T;
        }
        else
        {
            //started inside, now exitting
            tmax = hit.T;
        }

        tmax = Math.Min(tmax, oldtmax);

        // sample from exponential distrubution
        // cdf(L) = 1 - e^Ld
        // e^Ld = 1 + U
        // L = ln(1+U)/d

        double scatter = Math.Log(1 + Util.rng.NextDouble()) / density;
        double scatterT = scatter + tmin;
        if (tmin + scatter > tmax)
            return false;

        rec.T = scatterT;
        rec.Position = r.At(scatterT);
        rec.Material = material;
        rec.Normal = new(1, 0, 0);  // arbitrary
        rec.FrontFace = true;     // also arbitrary

        return true;
    }
}

internal class VolumeMaterial : IMaterial
{
    public Vector3 albedo;

    public VolumeMaterial(Vector3 albedo)
    {
        this.albedo = albedo;
    }

    public Vector3 Color(Ray rayIn, HitInfo hitInfo, Vector3 colorIn)
    {
        return colorIn * albedo;
    }

    public bool Scatter(Ray rayIn, HitInfo hitInfo, out Ray rayout)
    {
        rayout.Position = hitInfo.Position;
        rayout.Direction = Util.RandomInUnitSphere();
        return true;
    }
}