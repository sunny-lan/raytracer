using System.DoubleNumerics;

namespace raytracer;

internal struct AABB
{
    public readonly Vector3 Min, Max;

    public AABB(in Vector3 min, in Vector3 max)
    {
        Min = min;
        Max = max;
    }


    // solves t for At + B = k for each axis aligned plane
    static Vector3 PlanesIntersection(in Ray r, Vector3 k)
    {
        //t = (k - B) / A
        return (k - r.Position) / r.Direction;
    }

    public bool Hit(in Ray r, double tmin, double tmax)
    {
        Vector3 minIntersect = PlanesIntersection(r, Min);
        Vector3 maxIntersect = PlanesIntersection(r, Max);
        for(int i = 0; i < 3; i++)
        {
            double minI = minIntersect.Index(i), maxI = maxIntersect.Index(i);
            if(maxI < minI) Util.Swap(ref minI, ref maxI);
            tmin = Math.Max(tmin, minI);
            tmax = Math.Min(tmax, maxI);
        }
        return tmin < tmax;
    }

    public static AABB operator |(AABB a, AABB b)
    {
        return new(
            Util.Min(a.Min, b.Min),
            Util.Max(a.Max, b.Max)
        );
    }
}
