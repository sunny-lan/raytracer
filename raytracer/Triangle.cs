using System.DoubleNumerics;

namespace raytracer;

internal class Triangle : IHasBoundingBox
{
    readonly Matrix4x4 backward, forward;
    readonly Vector3 normal;
    readonly IMaterial material;
    readonly Vector2[] uvs;
    private readonly Vector3[] normals;
    readonly ITexture1? bumpMap;

    Triangle(
        in Vector3 a, 
        in Vector3 b, 
        in Vector3 c,
        in Vector3 normal,
        in Vector3 ba, 
        in Vector3 ca,
        in IMaterial material,
        in Vector2[] uvs,
        in Vector3[] normals,
        in ITexture1? bumpMap)
    {
        this.normal = normal;
        this.material = material;
        this.uvs = uvs;
        this.normals = normals;
        this.bumpMap = bumpMap;


        Vector3 free = new(1, 0, 0);
        if (Math.Abs(normal.Y) > Math.Abs(normal.X))
            free = new(0, 1, 0);
        if (Math.Abs(normal.Z) > Math.Abs(normal.Y) && Math.Abs(normal.Z) > Math.Abs(normal.X))
            free = new(0, 0, 1);

        // transforms a->0,0  b->1,0  c->0,1
        forward = new(
            ba.X, ca.X, free.X, a.X,
            ba.Y, ca.Y, free.Y, a.Y,
            ba.Z, ca.Z, free.Z, a.Z,
            0, 0, 0, 1
        );
        forward = Matrix4x4.Transpose(forward);

        if (!Matrix4x4.Invert(forward, out backward))
            throw new ArgumentException("Could not invert triangle transform");

        BoundingBox = new(
            Util.Min(a, b, c),
            Util.Max(a, b, c)
        );
    }

    public static Triangle? Make(
        in Vector3 a, 
        in Vector3 b, 
        in Vector3 c, 
        in IMaterial material,
        in Vector2[] uvs,
        in Vector3[] normals,
        in ITexture1? bumpMap = null
    )
    {
        // Baldwin intersection method
        Vector3 ba = b - a, ca = c - a;
        if (ba.NearZero() || ca.NearZero()) return null;

        Vector3 normal = Vector3.Cross(ba, ca);
        if (normal.NearZero()) return null;
        normal = normal.Normalized();

        return new(a, b, c, normal, ba, ca, material, uvs, normals, bumpMap);
    }

    public AABB BoundingBox { get; }

    public bool Hit(in Ray r, double tmin, double tmax, out HitInfo rec)
    {
        // transform ray to triangle space
        Vector3 rSt = r.Position, rEnd = r.Position + r.Direction;
        rSt = Vector3.Transform(rSt, backward);
        rEnd = Vector3.Transform(rEnd, backward);
        Vector3 dir = rEnd - rSt;

        // intersect with z=0
        double tsol = -rSt.Z / dir.Z;
        double x = tsol * dir.X + rSt.X,
               y = tsol * dir.Y + rSt.Y;

        rec = new();

        // Check within triangle
        if (
            tsol >= tmin && tsol <= tmax &&
            x is >= 0 and <= 1 &&
            y is >= 0 and <= 1 &&
            x + y <= 1)
        {
            rec.Material = material;
            rec.Position = r.At(tsol);
            rec.T = tsol;
            rec.uv = uvs[0] * (1 - x - y) + uvs[1] * x + uvs[2] * y;

            Vector3 normal;
            if (bumpMap is not null)
                normal = bumpMap.Normal(rec.uv);
            else
                normal = normals[0] * (1 - x - y) + normals[1] * x + normals[2] * y;
            rec.SetFaceNormal(r, normal);

            return true;
        }

        return false;
    }
}
