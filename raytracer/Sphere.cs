using System.DoubleNumerics;

namespace raytracer;

internal class Sphere : IHittable
{
    public Vector3 Center;
    public double Radius;
    public IMaterial Material;

    public Sphere(Vector3 center, double radius, IMaterial material)
    {
        Center = center;
        Radius = radius;
        Material = material;
    }

    public bool Hit(Ray r, double tmin, double tmax, out HitInfo rec)
    {
       // r.Position -= Center;

        // p(t) = Pos + t*Dir
        // p(t) . p(t) = r^2
        // t^2 Dir . Dir + 2t Pos . Dir + Pos . Pos - r^2 = 0 
        double a = Vector3.Dot(r.Direction, r.Direction),
               b = 2 * Vector3.Dot(r.Position-Center, r.Direction),
               c = Vector3.Dot(r.Position-Center, r.Position-Center) - Radius * Radius;

        double disc = b * b - 4 * a * c;
        rec = new HitInfo();

        if (disc > 0)
        {
            double tmp = Math.Sqrt(disc);
            double t1 = (-b - tmp) / (2 * a),
                   t2 = (-b + tmp) / (2 * a);

            double root;
            if (t1 >= tmin && t1 <= tmax)
                root = t1;
            else if (t2 >= tmin && t2 <= tmax)
                root = t2;
            else
                return false;

            rec.T = root;
            rec.Position = r.At(root);
            rec.SetFaceNormal(r, (rec.Position-Center) / Radius);
            rec.Material = Material;
            return true;
        }

        return false;
    }
}