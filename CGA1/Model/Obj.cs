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

        public void Transform(Matrix4x4 matrix, Matrix4x4 viewportMatrix, Matrix4x4 worldMatrix)
        {
            Vertices = Vertices.Select(v => Vector4.Transform(v, matrix) / v.W)
                .Zip(Vertices.Select(v => Vector4.Transform(v, viewportMatrix)), 
                    (v1, v2) => { v1.W = v2.W; return v1; })
                .ToList();

            Normals = Normals
                .Select(normal => Vector3.Normalize(Vector3.TransformNormal(normal, worldMatrix)))
                .ToList();
        }
    }
}
