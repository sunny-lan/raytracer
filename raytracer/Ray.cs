using System.DoubleNumerics;

namespace raytracer;

internal struct Ray
{
    public Vector3 Position;
    public Vector3 Direction;

    public Ray(Vector3 position, Vector3 forward)
    {
        Position = position;
        Direction = forward;
    }

    public Vector3 At(double t) => Position + Direction * t;

    // Finds t that p = P + Dt
    public double T(Vector3 p)
    {
        return Vector3.Dot(p - Position, Direction) / Direction.Length();
    }
}
