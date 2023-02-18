using System.DoubleNumerics;

namespace raytracer;

internal class Triangle : IHasBoundingBox
{
    readonly Matrix4x4 backward, forward;
    readonly Vector3 normal;
    readonly IMaterial material;
    readonly Vector2[] uvs;

    Triangle(
        in Matrix4x4 backward, 
        in Matrix4x4 forward, 
        in Vector3 normal, 
        in IMaterial material, 
        in AABB boundingBox, 
        in Vector2[] uvs)
    {
        this.backward = backward;
        this.forward = forward;
        this.normal = normal;
        this.material = material;
        BoundingBox = boundingBox;
        this.uvs = uvs;
    }

    public static Triangle? Make(
        Vector3 a, Vector3 b, Vector3 c, 
        IMaterial material,
        Vector2[] uvs
    )
    {
        // Baldwin intersection method
        Vector3 ba = b - a, ca = c - a;
        if (ba.NearZero() || ca.NearZero()) return null;

        Vector3 normal = Vector3.Cross(ba, ca);
        if (normal.NearZero()) return null;
        normal = normal.Normalized();

        Vector3 free = new(1, 0, 0);
        if (Math.Abs(normal.Y) > Math.Abs(normal.X))
            free = new(0, 1, 0);
        if (Math.Abs(normal.Z) > Math.Abs(normal.Y) && Math.Abs(normal.Z) > Math.Abs(normal.X))
            free = new(0, 0, 1);

        // transforms a->0,0  b->1,0  c->0,1
        Matrix4x4 forward = new(
            ba.X, ca.X, free.X, a.X,
            ba.Y, ca.Y, free.Y, a.Y,
            ba.Z, ca.Z, free.Z, a.Z,
            0, 0, 0, 1
        );
        forward = Matrix4x4.Transpose(forward);

        if (!Matrix4x4.Invert(forward, out var backward))
            throw new ArgumentException("Could not invert triangle transform");

        return new(backward, forward, normal, material, new(
            Util.Min(a, b, c),
            Util.Max(a, b, c)
        ), uvs);
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
            rec.Position = r.At(tsol);
            rec.T = tsol;
            rec.SetFaceNormal(r, normal);
            rec.Material = material;
            rec.uv = uvs[0] * (1 - x - y) + uvs[1] * x + uvs[2] * y;
            return true;
        }

        return false;
    }
}
