using ObjLoader.Loader.Data;
using ObjLoader.Loader.Loaders;
using System.DoubleNumerics;

namespace raytracer;
internal class MeshLoader
{
    public static IEnumerable<IHasBoundingBox> Load(string file, IMaterial defaultMaterial = null, Matrix4x4? transform = null)
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
            if (group.Material is not null && materials.ContainsKey(group.Material))
                material = materials[group.Material];

            IEnumerable<Triangle> face2Triangles(ObjLoader.Loader.Data.Elements.Face face)
            {

                Vector3 get(int idx)
                {
                    int vertexIndex = face[idx].VertexIndex - 1;

                    var vertex = result.Vertices[vertexIndex];

                    Vector3 pos = new(
                                                vertex.X,
                                                vertex.Y,
                                                vertex.Z
                                        );
                    if (transform is Matrix4x4 t)
                        pos = Vector3.Transform(pos, t);
                    return pos;
                }

                for (int i = 1; i + 1 < face.Count; i++)
                {
                    var triangle = Triangle.Make(
                       get(0), get(i), get(i + 1),
                       material,
                       Enumerable.Range(0, 3).Select(i =>
                       {
                           var tex = result.Textures[face[i].TextureIndex - 1];
                           return new Vector2(tex.X, tex.Y);
                       }).ToArray()
                    );
                    if (triangle is not null)
                        yield return triangle;
                }
            }

            return group.Faces.SelectMany(face2Triangles);
        });
    }
}
