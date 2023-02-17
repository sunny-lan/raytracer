using System.DoubleNumerics;

namespace raytracer;

internal class Triangle : IHasBoundingBox
{
    readonly Matrix4x4 backward, forward;
    readonly Vector3 normal;
    readonly IMaterial material;

    public Triangle(Vector3 a, Vector3 b, Vector3 c, IMaterial material)
    {
        // Baldwin intersection method
        Vector3 ba = b - a, ca = c - a;
        normal = Vector3.Cross(ba, ca);

        Vector3 free = new(1, 0, 0);
        if (Math.Abs(normal.Y) > Math.Abs(normal.X))
            free = new(0, 1, 0);
        if (Math.Abs(normal.Z) > Math.Abs(normal.Y) && Math.Abs(normal.Z) > Math.Abs(normal.X))
            free = new(0, 0, 1);

        forward = new(
            ba.X, ca.X, free.X, a.X,
            ba.Y, ca.Y, free.Y, a.Y,
            ba.Z, ca.Z, free.Z, a.Z,
            0, 0, 0, 1
        );

        if (!Matrix4x4.Invert(forward, out backward))
            throw new ArgumentException("Could not invert triangle transform");

        BoundingBox = new(
            Util.Min(a, b, c),
            Util.Max(a, b, c)
        );
        this.material = material;
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
            x is >= 0 and <= 1 &&
            y is >= 0 and <= 1 &&
            x + y <= 1)
        {
            rec.T = tsol; //does this work??
            rec.Position = r.At(rec.T);
            rec.SetFaceNormal(r, normal);
            rec.Material = material;
            return true;
        }

        return false;
    }
}
