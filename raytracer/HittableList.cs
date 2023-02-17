namespace raytracer;

internal class HittableList :IHittable
{
    readonly IEnumerable<IHittable> Objects;

    public HittableList(IEnumerable<IHittable> objects)
    {
        Objects = objects;
    }

    public bool Hit(in Ray r, double tmin, double tmax, out HitInfo rec)
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

internal class BoundedHittableList : HittableList, IHasBoundingBox
{
    private readonly IEnumerable<IHasBoundingBox> objects;

    public BoundedHittableList(IEnumerable<IHasBoundingBox> objects) : base(objects)
    {
        this.objects = objects;
    }

    public AABB BoundingBox
    {
        get
        {
            AABB result = objects.First().BoundingBox;
           

            foreach (var item in objects.Skip(1))
            {
                result |= item.BoundingBox;
            }

            return result;
        }

    }
}