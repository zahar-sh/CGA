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

        protected Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));
        }

        protected Vector4 GetFacePoint(IList<Vector3> face, int i)
        {
            var index = Convert.ToInt32(face[i].X);
            return Obj.Vertices[index];
        }

        protected Vector3 GetFaceTexture(IList<Vector3> face, int i)
        {
            var index = Convert.ToInt32(face[i].Y);
            return Obj.Textures[index];
        }

        protected Vector3 GetFaceNormal(IList<Vector3> face, int i)
        {
            var index = Convert.ToInt32(face[i].Z);
            return Obj.Normals[index];
        }

        protected Vector3 GetFaceNormal(IList<Vector3> face)
        {
            var p1 = GetFacePoint(face, 0);
            var p2 = GetFacePoint(face, 1);
            var p3 = GetFacePoint(face, 2);
            var v1 = new Vector3(p1.X, p1.Y, p1.Z);
            var v2 = new Vector3(p2.X, p2.Y, p2.Z);
            var v3 = new Vector3(p3.X, p3.Y, p3.Z);
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
                    foreach (var p in GetFaceSidePoints(face))
                    {
                        if (IsValidPoint(p.X, p.Y, p.Z))
                        {
                            DrawPoint(p.X, p.Y, Color);
                        }
                    }
                }
            });
        }

        protected IEnumerable<Point> GetFaceSidePoints(IList<Vector3> face)
        {
            var lastFaceIndex = face.Count - 1;
            return Enumerable
                .Range(0, lastFaceIndex)
                .Select(i => GetSidePoints(face, i, i + 1))
                .Append(GetSidePoints(face, 0, lastFaceIndex))
                .SelectMany(points => points);
        }

        protected IEnumerable<Point> GetSidePoints(IList<Vector3> face, int index1, int index2)
        {
            var p1 = GetPoint(face, index1);
            var p2 = GetPoint(face, index2);
            return GetLinePoints(p1, p2);
        }

        protected virtual Point GetPoint(IList<Vector3> face, int index)
        {
            var v = GetFacePoint(face, index);
            var x = Convert.ToInt32(v.X);
            var y = Convert.ToInt32(v.Y);
            return new Point(x, y, v.Z, 1, Vector3.Zero, Vector3.Zero);
        }

        protected IEnumerable<Point> GetLinePoints(Point p1, Point p2)
        {
            var x = p1.X;
            var y = p1.Y;
            var z = p1.Z;
            var nw = p1.NW;
            var n = p1.Normal;
            var t = p1.Texel;

            var dx = Math.Abs(p2.X - p1.X);
            var dy = Math.Abs(p2.Y - p1.Y);
            var dz = (p2.Z - p1.Z) / dy;
            var dnw = (p2.NW - p1.NW) / dy;
            var dn = (p2.Normal - p1.Normal) / dy;
            var dt = (p2.Texel - p1.Texel) / dy;

            var signX = Math.Sign(p2.X - p1.X);
            var signY = Math.Sign(p2.Y - p1.Y);

            var err = dx - dy;

            while (x != p2.X || y != p2.Y)
            {
                yield return new Point(x, y, z, nw, n, t);

                var err2 = err * 2;
                if (err2 > -dy)
                {
                    err -= dy;
                    x += signX;
                }
                if (err2 < dx)
                {
                    err += dx;
                    y += signY;
                    z += dz;
                    nw += dnw;
                    n += dn;
                    t += dt;
                }
            }
            yield return p2;
        }

        protected bool IsValidPoint(int x, int y, float z)
        {
            return x >= 0 && x < Buffer.Width &&
                y >= 0 && y < Buffer.Height &&
                z > 0 && z < 1;
        }

        protected void DrawPoint(int x, int y, Color color)
        {
            Buffer[x, y] = color;
        }
    }
}
