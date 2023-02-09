// See https://aka.ms/new-console-template for more information

using raytracer;
using System.DoubleNumerics;
using System.Drawing;

var width = 800;
var height = 400;
var aspRatio = width / (double)height;

Vector3 origin = new(3, 3,2), lookAt = new Vector3(0, 0, -1);
Camera camera = new(
    fov: 30,
    aspect: aspRatio,
    lookAt,
    up: new Vector3(0, 1, 0),
    origin,
    aperture: 0.1,
    focusDist: (lookAt-origin).Length()
);


using Bitmap bmp = new Bitmap(width, height);


HittableList world = new();

world.Objects.Add(new Sphere(new(0, 0, -1), 0.3, new Light(new(5, 0, 0.5))));
//world.Objects.Add(new Sphere(new(0.5, -0.1, -1.5), 0.6, new Metal(new(0.6, 0.6, 0.05))));
//world.Objects.Add(new Sphere(new(-3, 0, -2), 0.3, new Lambertian(new(0.01, 0.5, 0.05))));
world.Objects.Add(new Sphere(new(0, -100.5, -1), 100, new Lambertian(new(0.2, 0.2, 0.2))));

Vector3 center = new(0, 0.3, -3);
for(int i = 0; i < 25; i++)
{
    var offset = (Util.RandomV3() * 2 - Vector3.One) * new Vector3(3,  0.1,3);
    var color = Util.RandomV3();

    IMaterial mat = Util.rng.NextDouble() switch
    {
        < 0.4 => new Light(color),
        < 0.8 => new Lambertian(color),
        _ => new Metal(color),
    };

    var sphere = new Sphere(offset + center, Util.rng.NextDouble() * 0.5 + 0.1, mat);
    world.Objects.Add(Util.rng.NextDouble() switch
    {
        < 0.3 => new Volume(sphere, Util.rng.NextDouble()*10, new VolumeMaterial(color)),
        _ => sphere,
    });
}

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

    Vector3 unit_direction = r.Direction.Normalized();
    var t = 0.5 * (unit_direction.Y + 1.0);
    return (1.0 - t) * Vector3.One + t * new Vector3(0.5, 0.7, 1.0)*2;
}

int samples = 100;
int maxDepth = 10;

for (int y = 0; y < height; y++)
{
    Console.WriteLine($"Line {y}");
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

        bmp.SetPixel(x, y, Color.FromArgb(
            (int)Math.Round(color.X),
            (int)Math.Round(color.Y),
            (int)Math.Round(color.Z)
        ));
    }
}

bmp.Save("out.bmp");