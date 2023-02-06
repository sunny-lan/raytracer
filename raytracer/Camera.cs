using System.DoubleNumerics;

namespace raytracer;

internal class Camera
{
    public readonly Vector3 origin;
    readonly Vector3 left;
    readonly Vector3 up;
    readonly Vector3 leftNorm;
    readonly Vector3 upNorm;
    readonly Vector3 bottomLeft;
    readonly double lensRadius;



    public Camera(
        double fov, double aspect,
        Vector3 lookAt, Vector3 up, Vector3 origin,
        double aperture,
        double focusDist
    )
    {
        fov = fov.ToRadians();
        var h = Math.Tan(fov / 2);
        var vpHeight = h*2;
        var vpWidth = vpHeight * aspect;

        this.origin = origin;

        var lookDir = (origin-lookAt).Normalized();
        this.leftNorm = Vector3.Cross(up, lookDir).Normalized();
        this.upNorm = Vector3.Cross(lookDir, leftNorm).Normalized();

        this.left = focusDist* leftNorm * vpWidth;
        this.up = focusDist* upNorm * vpHeight;

        bottomLeft = origin - lookDir*focusDist - left / 2 - up / 2;
        lensRadius = aperture / 2;

    }

    public Ray GetRay(double u, double v)
    {
        var rd = Util.RandomInCircle() * lensRadius;
        var offset = leftNorm * rd.X + upNorm * rd.Y;
        var trueOrigin = offset + origin;

        var vpPoint = bottomLeft + left * u + up * v;
        return new Ray(trueOrigin, vpPoint - trueOrigin);
    }
}