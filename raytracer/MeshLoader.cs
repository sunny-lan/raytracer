using ObjLoader.Loader.Loaders;
using System.DoubleNumerics;

namespace raytracer;

internal class MeshLoader
{
    public static IEnumerable<IHasBoundingBox> Load(string file)
    {
        var objLoaderFactory = new ObjLoaderFactory();
        var objLoader = objLoaderFactory.Create();
        using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        var result = objLoader.Load(fileStream);
        return result.Groups.SelectMany(
            group => group.Faces.Where(face => face.Count == 3).Select(
                face =>
                {
                    Vector3 get(int idx)
                    {
                        int vertexIndex = face[idx].VertexIndex - 1;

                        var vertex = result.Vertices[vertexIndex];
                        return new(
                             vertex.X,
                             vertex.Y,
                             vertex.Z
                        );
                    }
                    return Triangle.Make(
                        get(0), get(1), get(2),
                        new Metal(new Vector3(0.5, 0.6, 0.8))
                    );
                }
            ).Where(x => x is not null).Cast<Triangle>()
        );
    }
}
