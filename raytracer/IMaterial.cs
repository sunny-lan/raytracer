using System.DoubleNumerics;

namespace raytracer;

internal interface IMaterial
{
    bool Scatter(in Ray rayIn, in HitInfo hitInfo, out Ray rayout);

    Vector3 Color(in Ray rayIn, in HitInfo hitInfo, in Vector3 colorIn);
}
