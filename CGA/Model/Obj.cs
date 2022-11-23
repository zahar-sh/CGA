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
            var matrix = viewportMatrix * projectionMatrix * viewMatrix * modelMatrix;
            var vertices = Vertices
                .Select(v => Vector4.Transform(v, matrix) / v.W)
                .ToList();
            return new Obj()
            {
                Vertices = vertices,
                Textures = Textures,
                Normals = Normals,
                Faces = Faces
            };
        }
    }
}
