namespace raytracer;

internal class HittableList :IHittable
{
    public readonly List<IHittable> Objects = new();

    public bool Hit(Ray r, double tmin, double tmax, out HitInfo rec)
    {
        IHittable? closest = null;
        double minDist = tmax;
        rec = new();

        foreach(var obj in Objects)
        {
            if(obj.Hit(r, tmin, minDist, out var tmpRec)) {
                closest = obj;
                minDist = tmpRec.T;
                rec= tmpRec;
            }
        }

        return closest != null;
    }
}