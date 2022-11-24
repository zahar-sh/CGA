using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGA.Model
{
    public class Obj
    {
        public IList<Vector4> Vertices { get; set; }

        public IList<Vector3> Textures { get; set; }

        public IList<Vector3> Normals { get; set; }

        public IList<IList<Vector3>> Faces { get; set; }

        public Obj Transform(Matrix4x4 viewportMatrix, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix, Matrix4x4 modelMatrix)
        {
            var worldProjectionMatrix = modelMatrix * viewMatrix * projectionMatrix;
            var vertices = Vertices
                .Select(v => Vector4.Transform(v, worldProjectionMatrix))
                .Select(v =>
                {
                    var w = v.W;
                    var v2 = Vector4.Transform(v / w, viewportMatrix);
                    v2.W = w;
                    return v2;
                })
                .ToList();
            var normals = Normals
                .Select(normal => Vector3.Normalize(Vector3.TransformNormal(normal, modelMatrix)))
                .ToList();
            return new Obj()
            {
                Vertices = vertices,
                Textures = Textures.ToList(),
                Normals = normals,
                Faces = Faces.ToList()
            };
        }

        private IEnumerable<Vector3> CreateFace(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            yield return v1;
            yield return v2;
            yield return v3;
        }

        public IEnumerable<IEnumerable<Vector3>> GetTriangleFaces()
        {
            return Faces
                .SelectMany(face => Enumerable
                    .Range(1, face.Count - 2)
                    .Select(i => CreateFace(face[0], face[i], face[i + 1])));
        }

        public override string ToString()
        {
            return $"Vertices: {string.Join(",", Vertices)}\r\n" +
                $"Textures: {string.Join(",", Textures)}\r\n" +
                $"Normals:{string.Join(",", Normals)}\r\n" +
                $"Faces:{string.Join(",", Faces.Select(face => $"[{string.Join(",", face)}"))}]";
        }
    }
}
