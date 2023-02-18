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
        Dictionary<Material, ITexture1> bumpMaps = new();
        foreach (var material in result.Materials)
        {
            materials[material] = new Lambertian(new ImageTexture3(material.DiffuseTextureMap));
            if(material.BumpMap?.Length > 0)
                bumpMaps[material] = new ImageTexture1(material.BumpMap);
        }


        return result.Groups.SelectMany(group =>
        {
            IMaterial material = defaultMaterial;
            ITexture1? bumpMap = null;
            if (group.Material is not null)
            {
                if (materials.ContainsKey(group.Material))
                    material = materials[group.Material];

                if (bumpMaps.ContainsKey(group.Material))
                    bumpMap = bumpMaps[group.Material];
            }

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
                    var triangleIdxs = new int[] { 0, i, i + 1 };
                    var triangle = Triangle.Make(
                       a: get(0), b: get(i), c: get(i + 1),
                       material,
                       uvs: triangleIdxs.Select(i =>
                       {
                           var tex = result.Textures[face[i].TextureIndex - 1];
                           return new Vector2(tex.X, tex.Y);
                       }).ToArray(),
                       normals: triangleIdxs.Select(i =>
                       {
                           var n = result.Normals[face[i].NormalIndex - 1];
                           return new Vector3(n.X, n.Y, n.Z);
                       }).ToArray(),
                       bumpMap
                    );
                    if (triangle is not null)
                        yield return triangle;
                }
            }

            return group.Faces.SelectMany(face2Triangles);
        });
    }
}
