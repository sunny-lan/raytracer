using System.DoubleNumerics;

namespace raytracer;

internal static class Util
{
    public static double ToRadians(this double degree)
    {
        return Math.PI * degree / 180.0;
    }
    public static Vector3 Normalized(this Vector3 v)
    {
        return v / v.Length();
    }
    public static bool NearZero(this Vector3 v, double eps=1e-8)
    {
        return (Math.Abs(v.X) < eps)
            && (Math.Abs(v.Y) < eps)
            && (Math.Abs(v.Z) < eps);
    }
    public static Vector3 Sqrt(this Vector3 v)
    {
        return new(
            Math.Sqrt(v.X),
            Math.Sqrt(v.Y),
            Math.Sqrt(v.Z)
        );
    }


    public static readonly Random rng = new Random();

    public static Vector3 RandomV3()
    {
        return new Vector3(
            rng.NextDouble(),
            rng.NextDouble(),
            rng.NextDouble()
        );
    }

    public static Vector3 RandomInUnitSphere()
    {
        while (true)
        {
            var res = RandomV3()*2-Vector3.One;
            if (res.LengthSquared() <= 1) return res;
        }
    }
    public static Vector2 RandomV2()
    {
        return new Vector2(
            rng.NextDouble(),
            rng.NextDouble()
        );
    }

    internal static Vector2 RandomInCircle()
    {
        while (true)
        {
            var res = RandomV2() * 2 - Vector2.One;
            if (res.LengthSquared() <= 1) return res;
        }
    }
}
