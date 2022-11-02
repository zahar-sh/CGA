using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Numerics;

namespace CGA1.Model
{
    public class Obj
    {
        public IList<Vector4> Points { get; }
        public IList<Vector3> Textures { get; }
        public IList<Vector3> Normals { get; }
        public IList<IList<Vector3>> Faces { get; }

        public Obj(IList<Vector4> points, IList<Vector3> textures, IList<Vector3> normals, IList<IList<Vector3>> faces)
        {
            Points = Guard.Against.Null(points, nameof(points));
            Textures = Guard.Against.Null(textures, nameof(textures));
            Normals = Guard.Against.Null(normals, nameof(normals));
            Faces = Guard.Against.Null(faces, nameof(faces));
        }
    }
}
