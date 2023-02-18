using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.DoubleNumerics;

namespace raytracer;

public class ImageTexture : ITexture
{
    private Image<Rgb48> image;

    public ImageTexture(string file)
    {
        image = Image.Load<Rgb48>(file);
    }

    public Vector3 Sample(in Vector2 uv)
    {
        Vector2 coords = uv * new Vector2(image.Width, image.Height);
        var pixel = image[
            (int)Math.Round(coords.X),
            (int)Math.Round(image.Height - coords.Y)
        ].ToScaledVector4();
        return new Vector3(pixel.X, pixel.Y, pixel.Z);
    }
}
