using System.DoubleNumerics;

namespace raytracer;

internal interface IMaterial
{
    bool Scatter(Ray rayIn, HitInfo hitInfo, out Ray rayout);

    Vector3 Color(Ray rayIn, HitInfo hitInfo, Vector3 colorIn);
}
