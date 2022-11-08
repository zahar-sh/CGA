using System.Collections.Generic;
using System.Numerics;

namespace CGA1.Model
{
    public class Obj
    {
        public IList<Vector4> Vertices { get; set; }
        public IList<Vector3> Textures { get; set; }
        public IList<Vector3> Normals { get; set; }
        public IList<IList<Vector3>> Faces { get; set; }
    }
}
