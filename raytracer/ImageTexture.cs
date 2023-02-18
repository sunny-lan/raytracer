using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.DoubleNumerics;

namespace raytracer;
public class ImageTextureA<In> : ITexture<In> where In : unmanaged, IPixel<In>
{
    private Image<In> image;

    public ImageTextureA(string file)
    {
        image = Image.Load<In>(file);
    }

    public In Sample(in Vector2 uv, bool raw )
    {
        Vector2 coords = uv;
        Vector2 sz = new Vector2(image.Width, image.Height);
        if (!raw)
        {
            coords *= sz;
        }
        coords.Y = image.Height - coords.Y;
        return image[
            (int)Math.Clamp( Math.Round(coords.X),0,image.Width-1),
            (int)Math.Clamp( Math.Round(coords.Y),0,image.Height-1)
        ];
    }
}
public class ImageTexture3 : ImageTextureA<Rgb48>, ITexture3
{
    public ImageTexture3(string file) : base(file)
    {
    }

    Vector3 ITexture<Vector3>.Sample(in Vector2 uv, bool raw)
    {
        var val = Sample(uv, raw).ToScaledVector4();
        return new Vector3(val.X, val.Y, val.Z);
    }
}
public class ImageTexture1 : ImageTextureA<L16>, ITexture1
{
    public ImageTexture1(string file) : base(file)
    {
    }

    double ITexture<double>.Sample(in Vector2 uv, bool raw)
    {
        return Sample(uv, raw).ToScaledVector4().X;
    }
}
