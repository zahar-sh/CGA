using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.Model
{
    public class Bresenham : IObjPainter
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

        protected static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));
        }

        protected Vector4 GetFacePoint(IList<Vector3> face, int i)
        {
            int index = Convert.ToInt32(face[i].X);
            return Obj.Vertices[index];
        }

        protected Vector3 GetFaceNormal(IList<Vector3> face)
        {
            var v1 = GetFacePoint(face, 0).ToVector3();
            var v2 = GetFacePoint(face, 1).ToVector3();
            var v3 = GetFacePoint(face, 2).ToVector3();
            return GetNormal(v1, v2, v3);
        }

        protected bool IsFaceVisible(IList<Vector3> face)
        {
            var normal = GetFaceNormal(face);
            return normal.Z < 0;
        }

        public virtual void DrawModel()
        {
            _ = Parallel.ForEach(Obj.Faces, face =>
            {
                if (IsFaceVisible(face))
                {
                    DrawFace(face, Color);
                }
            });
        }

        protected void DrawFace(IList<Vector3> face, Color color)
        {
            var lastFaceIndex = face.Count - 1;
            for (int i = 0; i < lastFaceIndex; i++)
            {
                DrawSide(face, i, i + 1, color);
            }
            DrawSide(face, 0, lastFaceIndex, color);
        }

        protected void DrawSide(IList<Vector3> face, int index1, int index2, Color color)
        {
            var v1 = GetFacePoint(face, index1);
            var v2 = GetFacePoint(face, index2);

            var x1 = Convert.ToInt32(v1.X);
            var y1 = Convert.ToInt32(v1.Y);
            var z1 = v1.Z;

            var x2 = Convert.ToInt32(v2.X);
            var y2 = Convert.ToInt32(v2.Y);
            var z2 = v2.Z;
            DrawLine(x1, y1, z1, x2, y2, z2, color);
        }

        protected void DrawLine(int x1, int y1, float z1, int x2, int y2, float z2, Color color)
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
                DrawPoint(x, y, z, color);

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
            DrawPoint(x2, y2, z2, color);
        }

        protected virtual void DrawPoint(int x, int y, float z, Color color)
        {
            if (x >= 0 && x < Buffer.Width &&
                y >= 0 && y < Buffer.Height &&
                z > 0 && z < 1)
            {
                Buffer[x, y] = color;
            }
        }
    }
}
