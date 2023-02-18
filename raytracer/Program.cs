// See https://aka.ms/new-console-template for more information

using raytracer;
using System.DoubleNumerics;
using System.Drawing;

var width = 800;
var height = 400;
var aspRatio = width / (double)height;

Vector3 origin = new(-5, 3, -5), lookAt = new Vector3(0, 0.5, 0);
Camera camera = new(
    fov: 30,
    aspect: aspRatio,
    lookAt,
    up: new Vector3(0, 1, 0),
    origin,
    aperture: 0.1,
    focusDist: (lookAt - origin).Length()
);


using Bitmap bmp = new Bitmap(width, height);


List<IHasBoundingBox> objects = new();

objects.Add(new Sphere(new(0, -100.5, -1), 100, new Lambertian(new Vector3(0.2, 0.2, 0.2))));
objects.Add(new Sphere(new(0, 0, 0), 1, new Light(new Vector3(0.5, 0.5, 10))));

void genrandom()
{
    objects.Add(new Sphere(new(0, 0, -1), 0.3, new Light(new Vector3(5, 0, 0.5))));
    objects.Add(new Sphere(new(0.5, -0.1, -1.5), 0.6, new Metal(new Vector3(0.6, 0.6, 0.05))));
    objects.Add(new Sphere(new(-3, 0, -2), 0.3, new Lambertian(new Vector3(0.01, 0.5, 0.05))));
    //objects.Add(new BoundedVolume(new Sphere(new(0, 0, 0), 10, null), 0.1, new VolumeMaterial(new(0.8, 0.8, 0.8))));
    Vector3 center = new(0, 0.3, -3);
    for (int i = 0; i < 25; i++)
    {
        var offset = (Util.RandomV3() * 2 - Vector3.One) * new Vector3(3, 0.1, 3);
        var color = Util.RandomV3();

        IMaterial mat = Util.rng.NextDouble() switch
        {
            < 0.4 => new Light(color),
            < 0.8 => new Lambertian(color),
            _ => new Metal(color),
        };

        var sphere = new Sphere(offset + center, Util.rng.NextDouble() * 0.5 + 0.1, mat);
        objects.Add(Util.rng.NextDouble() switch
        {
            < 0.3 => new BoundedVolume(sphere, Util.rng.NextDouble() * 10, new VolumeMaterial(color)),
            _ => sphere,
        });
    }
}
//genrandom();
//objects.AddRange(MeshLoader.Load(
//    "Klee/Body.obj", 
//    defaultMaterial: new Lambertian(new ImageTexture("Klee/Avatar_Loli_Catalyst_Klee_Tex_Body_Diffuse.png")),
//    transform: Matrix4x4.CreateTranslation(0,1,0)
//));
objects.AddRange(MeshLoader.Load(
    "12221_Cat_v1_l3.obj",
    transform: 
        Matrix4x4.CreateRotationZ(Math.PI / 2) 
        * Matrix4x4.CreateRotationX(-Math.PI / 2) 
        * Matrix4x4.CreateScale(0.1)
));
//objects.Add(Triangle.Make(new(0, 1, -1),new(1,0,-1), new(0, 0, -1), new Lambertian(Vector3.One*2))!);
//objects.Add(Triangle.Make(new(0, -1, -2), new(-1,0,-2), new(0, 0, -2), new Lambertian(Vector3.One*0.5))!);
//objects.Add(Triangle.Make(new(0, 1, 0),new(1, 0, 0), new(0, 0, 0), new Lambertian(Vector3.One*2))!);
//objects.Add(new BoundedVolume(
//    new BVH(MeshLoader.Load("Klee/Body.obj").ToArray()),
//    10,
//    new VolumeMaterial(new(0.5, 0.2, 0.1))
//));
IHittable world = new BVH(objects.ToArray());

Vector3 RayColor(Ray r, int depth)
{
    if (depth <= 0) return Vector3.Zero;

    if (world.Hit(r, 0.001, double.PositiveInfinity, out var hit))
    {
        Vector3 color;
        if (hit.Material.Scatter(r, hit, out var scatteredRay))
        {
            color = RayColor(scatteredRay, depth - 1);
        }
        else
        {
            color = Vector3.Zero;
        }

        return hit.Material.Color(r, hit, color);
    }

    //Vector3 sunDir = new(0.2, -1, 0), sunCol = new(1, 1, 0.2), skyCol = new(0.1, 0.05, 0.5);
    //double strength = Vector3.Dot(-r.Direction, sunDir) / sunDir.Length() / r.Direction.Length();
    //strength = Math.Clamp(strength, 0, 1);
    //return strength * sunCol;

    var t = 0.5 * (r.Direction.Y / r.Direction.Length() + 1.0) ;
    return (1.0 - t) * new Vector3(1.0, 1.0, 1.0) + t * new Vector3(0.5, 0.7, 1.0);
}

int samples = 100;
int maxDepth = 10;
int pixelsRendered = 0;
Color[,] ans = new Color[width, height];

Parallel.For(0, height, y => 
{
    int prevProg = 0;
    void updateProgress(int v)
    {
        Interlocked.Add(ref pixelsRendered, v - prevProg);
        prevProg = v;
        Console.WriteLine(pixelsRendered / (double)(width * height));
    }

    for (int x = 0; x < width; x++)
    {
        Vector3 avgCol = Vector3.Zero;

        for (int sample = 0; sample < samples; sample++)
        {
            var ray = camera.GetRay(
                (x + Util.rng.NextDouble()) / (double)(width - 1),
                (height - y - Util.rng.NextDouble()) / (double)(height - 1)
            );
            avgCol += RayColor(ray, maxDepth);
        }
        avgCol = (avgCol / samples).Sqrt();
        var color = Vector3.Clamp(avgCol * 256, Vector3.Zero, new(255, 255, 255));

        ans[x, y] = Color.FromArgb(
                (int)Math.Round(color.X),
                (int)Math.Round(color.Y),
                (int)Math.Round(color.Z)
            );
       
    }
    updateProgress(width);

});

for(int x=0;x<width;x++)for(int y=0;y<height;y++)
    bmp.SetPixel(x, y, ans[x,y]);

bmp.Save("out.bmp");
