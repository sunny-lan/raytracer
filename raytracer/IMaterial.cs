using System.DoubleNumerics;

namespace raytracer;

internal interface IMaterial
{
    bool Scatter(in Ray rayIn, in HitInfo hitInfo, out Ray rayout);

    Vector3 Reflect(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn, in Ray raySrc) { return Vector3.Zero; }

    Vector3 Emit(in Ray rayIn, in HitInfo hitInfo) { return Vector3.Zero; }
}
