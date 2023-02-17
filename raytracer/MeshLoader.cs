using ObjLoader.Loader.Data;
using ObjLoader.Loader.Loaders;
using System.DoubleNumerics;

namespace raytracer;
internal class MeshLoader
{
    public static IEnumerable<IHasBoundingBox> Load(string file, IMaterial defaultMaterial)
    {
        var objLoaderFactory = new ObjLoaderFactory();
        var objLoader = objLoaderFactory.Create();
        using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        var result = objLoader.Load(fileStream);

        Dictionary<Material, IMaterial> materials = new();
        foreach (var material in result.Materials)
        {
            materials[material] = new Lambertian(new ImageTexture(material.DiffuseTextureMap));
        }

        return result.Groups.SelectMany(group =>
        {
            IMaterial material = defaultMaterial;
            if(group.Material is not null && materials.ContainsKey(group.Material))
                material = materials[group.Material];

            return group.Faces.Where(face => face.Count == 3).Select(face =>
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
                    material,
                    Enumerable.Range(0, 3).Select(i =>
                    {
                        var tex = result.Textures[face[i].TextureIndex - 1];
                        return new Vector2(tex.X, tex.Y);
                    }).ToArray()
                );
            }).Where(x => x is not null).Cast<Triangle>();
        });
    }
}
