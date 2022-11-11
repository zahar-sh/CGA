using System.Numerics;

namespace CGA1.Model
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
