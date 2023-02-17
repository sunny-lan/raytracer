using System.DoubleNumerics;

namespace raytracer;

struct HitInfo
{
    public double T;
    public Vector3 Normal;
    public Vector3 Position;
    public bool FrontFace;
    public IMaterial Material;
    public Vector2 uv;

    public void SetFaceNormal(in Ray r, in Vector3 outNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outNormal) < 0;
        Normal = FrontFace ? outNormal : -outNormal;
    }
}

internal interface IHittable
{
    bool Hit(in Ray r, double tmin, double tmax, out HitInfo rec);
}

internal interface IHasBoundingBox : IHittable
{
    public AABB BoundingBox { get; }
}