namespace raytracer;

internal class BVH : IHasBoundingBox
{
    public readonly IHasBoundingBox left, right;

    public BVH(Span<IHasBoundingBox> objs)
    {
        if (objs.Length == 1)
        {
            left = right = objs[0];
        }
        else if (objs.Length == 2)
        {
            left = objs[0];
            right = objs[1];
        }
        else
        {
            int axis = Util.rng.Next(0, 3);
            objs.Sort((a, b) => a.BoundingBox.Min.Index(axis).CompareTo(b.BoundingBox.Min.Index(axis)));

            int mid = objs.Length / 2;
            left = new BVH(objs.Slice(0, mid));
            right = new BVH(objs.Slice(mid));
        }

        BoundingBox = left.BoundingBox | right.BoundingBox;
    }

    public AABB BoundingBox { get; private set; }

    public bool Hit(in Ray r, double tmin, double tmax, out HitInfo rec)
    {
        if (!BoundingBox.Hit(r, tmin, tmax))
        {
            rec = new();
            return false;
        }

        bool hit = left.Hit(r, tmin, tmax, out rec);
        bool rhit = right.Hit(r, tmin, hit ? rec.T : tmax, out var tmp);
        if(rhit) rec = tmp;
        return hit || rhit;
    }
}
