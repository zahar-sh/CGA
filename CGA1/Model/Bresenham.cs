using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

namespace CGA1.Model
{
    public class Bresenham
    {
        public Obj Obj { get; }

        public WritableImage Image { get; }

        public Color Color { get; }

        public Bresenham(Obj obj, WritableImage image, Color color)
        {
            Obj = obj;
            Image = image;
            Color=color;
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
            foreach (var face in Obj.Faces)
            {
                if (IsFaceVisible(face))
                {
                    DrawFace(face);
                }
            }
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

        private void DrawLine(Vector3 src, Vector3 desc)
        {
            var dx = Math.Abs(desc.X - src.X);
            var dy = Math.Abs(desc.Y - src.Y);
            var dz = Math.Abs(desc.Z - src.Z);

            var signX = src.X < desc.X ? 1 : -1;
            var signY = src.Y < desc.Y ? 1 : -1;
            float signZ = src.Z < desc.Z ? 1 : -1;

            var p = src;

            float curZ = src.Z;
            float deltaZ = dz / dy;

            var err = dx - dy;

            while (p.X != desc.X || p.Y != desc.Y)
            {
                DrawPoint((int)p.X, (int)p.Y, curZ);

                var err2 = err * 2;
                if (err2 > -dy)
                {
                    p.X += signX;
                    err -= dy;
                }
                if (err2 < dx)
                {
                    p.Y += signY;
                    err += dx;
                    curZ += signZ * deltaZ;
                }
            }
            DrawPoint((int)desc.X, (int)desc.Y, desc.Z);
        }

        private void DrawPoint(int x, int y, float z)
        {
            if (x > 0 && x < Image.Width &&
                y > 0 && y < Image.Height &&
                z > 0 && z < 1)
            {
                Image[x, y] = Color;
            }
        }
    }
}
