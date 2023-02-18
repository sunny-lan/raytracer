using System.DoubleNumerics;

namespace raytracer;

internal class Light : Lambertian, IMaterial
{
    public Light(Vector3 color):base(color)
    {
    }

    Vector3 IMaterial.Emit(in Ray rayIn, in HitInfo hitInfo)
    {
        return albedo.Sample(hitInfo.uv);
    }
}