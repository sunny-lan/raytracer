using System.DoubleNumerics;

namespace raytracer;

struct HitInfo
{
    public double T;
    public Vector3 Normal;
    public Vector3 Position;
    public bool FrontFace;
    public IMaterial Material;

    public void SetFaceNormal(Ray r, Vector3 outNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outNormal) < 0;
        Normal = FrontFace ? outNormal : -outNormal;
    }
}

internal interface IHittable
{
    bool Hit(Ray r, double tmin, double tmax, out HitInfo rec);
}
