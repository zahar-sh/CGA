using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGA1.Model
{
    public class Obj
    {
        public IList<Vector4> Vertices { get; set; }

        public IList<Vector3> Textures { get; set; }

        public IList<Vector3> Normals { get; set; }

        public IList<IList<Vector3>> Faces { get; set; }

        public Obj Transform(Matrix4x4 viewportMatrix, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix, Matrix4x4 modelMatrix)
        {
            var matrix = modelMatrix * viewMatrix * projectionMatrix;
            var vertices = Vertices
                .Select(v =>
                {
                    var result = Vector4.Transform(Vector4.Transform(v, matrix) / v.W, viewportMatrix);
                    result.W = v.W;
                    return result;
                })
                .ToList();
            var textures = Textures.Select(v => v).ToList();
            var normals = Normals
                .Select(v =>
                {
                    return Vector3.Normalize(Vector3.TransformNormal(v, modelMatrix));
                })
                .ToList();
            var faces = Faces.Select(v => v).ToList();
            return new Obj()
            {
                Vertices = vertices,
                Textures = textures,
                Normals = normals,
                Faces = faces
            };
        }
    }
}
