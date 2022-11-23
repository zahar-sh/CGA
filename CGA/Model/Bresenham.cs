using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA1.Model
{
    public class Bresenham
    {
        public Obj Obj { get; }

        public ColorBuffer Buffer { get; }

        public Color Color { get; }

        public Bresenham(Obj obj, ColorBuffer buffer, Color color)
        {
            Obj = obj;
            Buffer = buffer;
            Color = color;
        }

        private static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));
        }

        private Vector4 GetFacePoint(IList<Vector3> face, int i)
        {
            int index = (int)face[i].X;
            return Obj.Vertices[index];
        }

        private Vector3 GetFaceNormal(IList<Vector3> face)
        {
            var v1 = GetFacePoint(face, 0).ToVector3();
            var v2 = GetFacePoint(face, 1).ToVector3();
            var v3 = GetFacePoint(face, 2).ToVector3();
            return GetNormal(v1, v2, v3);
        }

        private bool IsFaceVisible(IList<Vector3> face)
        {
            var normal = GetFaceNormal(face);
            return normal.Z < 0;
        }

        public void DrawModel()
        {
            _ = Parallel.ForEach(Obj.Faces.Where(IsFaceVisible), DrawFace);
        }

        private void DrawFace(IList<Vector3> face)
        {
            var lastFaceIndex = face.Count - 1;
            for (int i = 0; i < lastFaceIndex; i++)
            {
                DrawSide(face, i, i + 1);
            }
            DrawSide(face, 0, lastFaceIndex);
        }

        private void DrawSide(IList<Vector3> face, int index1, int index2)
        {
            var v1 = GetFacePoint(face, index1).ToVector3();
            var v2 = GetFacePoint(face, index2).ToVector3();
            DrawLine(v1, v2);
        }

        private void DrawLine(Vector3 v1, Vector3 v2)
        {
            int dx = (int)Math.Abs(v2.X - v1.X);
            int dy = (int)Math.Abs(v2.Y - v1.Y);
            float dz = Math.Abs(v2.Z - v1.Z);

            int signX = v1.X < v2.X ? 1 : -1;
            int signY = v1.Y < v2.Y ? 1 : -1;
            int signZ = v1.Z < v2.Z ? 1 : -1;

            int x = (int)v1.X;
            int y = (int)v1.Y;
            float z = v1.Z;

            float deltaZ = dz / dy;

            int err = dx - dy;

            while (x != (int)v2.X || y != (int)v2.Y)
            {
                DrawPoint(x, y, z);

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
            DrawPoint((int)v2.X, (int)v2.Y, v2.Z);
        }

        private void DrawPoint(int x, int y, float z)
        {
            if (x >= 0 && x < Buffer.Width &&
                y >= 0 && y < Buffer.Height &&
                z > 0 && z < 1)
            {
                Buffer[x, y] = Color;
            }
        }
    }
}
