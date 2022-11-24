using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.Model
{
    public class Bresenham
    {
        public Bresenham(Obj obj, ColorBuffer buffer, Color color)
        {
            Obj = obj;
            Buffer = buffer;
            Color = color;
        }

        public Obj Obj { get; }

        public ColorBuffer Buffer { get; }

        public Color Color { get; }

        private Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));
        }

        private Vector4 GetFacePoint(IList<Vector3> face, int i)
        {
            int index = Convert.ToInt32(face[i].X);
            return Obj.Vertices[index];
        }

        private Vector3 GetFaceNormal(IList<Vector3> face)
        {
            var p1 = GetFacePoint(face, 0);
            var p2 = GetFacePoint(face, 1);
            var p3 = GetFacePoint(face, 2);
            var v1 = new Vector3(p1.X, p1.Y, p1.Z);
            var v2 = new Vector3(p2.X, p2.Y, p2.Z);
            var v3 = new Vector3(p3.X, p3.Y, p3.Z);
            return GetNormal(v1, v2, v3);
        }

        public bool IsFaceVisible(IList<Vector3> face)
        {
            var normal = GetFaceNormal(face);
            return normal.Z < 0;
        }

        public virtual void DrawModel()
        {
            var points = Obj.Faces
                .Where(IsFaceVisible)
                .SelectMany(GetFacePoints)
                .Where(point => IsValidPoint(point.X, point.Y, point.Z));
            _ = Parallel.ForEach(points, point =>
            {
                DrawPoint(point.X, point.Y, Color);
            });
        }

        public IEnumerable<(int X, int Y, float Z)> GetFacePoints(IList<Vector3> face)
        {
            var lastFaceIndex = face.Count - 1;
            return Enumerable
                .Range(0, lastFaceIndex)
                .Select(i => GetSidePoints(face, i, i + 1))
                .Append(GetSidePoints(face, 0, lastFaceIndex))
                .SelectMany(points => points);
        }

        private IEnumerable<(int X, int Y, float Z)> GetSidePoints(IList<Vector3> face, int index1, int index2)
        {
            var v1 = GetFacePoint(face, index1);
            var v2 = GetFacePoint(face, index2);

            var x1 = Convert.ToInt32(v1.X);
            var y1 = Convert.ToInt32(v1.Y);
            var z1 = v1.Z;

            var x2 = Convert.ToInt32(v2.X);
            var y2 = Convert.ToInt32(v2.Y);
            var z2 = v2.Z;
            return GetLinePoints(x1, y1, z1, x2, y2, z2);
        }

        public IEnumerable<(int X, int Y, float Z)> GetLinePoints(int x1, int y1, float z1, int x2, int y2, float z2)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            float dz = Math.Abs(z2 - z1);

            int signX = Math.Sign(x2 - x1);
            int signY = Math.Sign(y2 - y1);
            int signZ = Math.Sign(z2 - z1);

            int x = x1;
            int y = y1;
            float z = z1;

            float deltaZ = dz / dy;

            int err = dx - dy;

            while (x != x2 || y != y2)
            {
                yield return (x, y, z);

                int err2 = err * 2;
                if (err2 > -dy)
                {
                    err -= dy;
                    x += signX;
                }
                if (err2 < dx)
                {
                    err += dx;
                    y += signY;
                    z += signZ * deltaZ;
                }
            }
            yield return (x2, y2, z2);
        }

        public bool IsValidPoint(int x, int y, float z)
        {
            return x >= 0 && x < Buffer.Width &&
                y >= 0 && y < Buffer.Height &&
                z > 0 && z < 1;
        }

        public void DrawPoint(int X, int Y, Color color)
        {
            Buffer[X, Y] = color;
        }
    }
}
