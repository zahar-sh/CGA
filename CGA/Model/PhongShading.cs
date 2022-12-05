using System.Collections.Generic;
using System.Linq;
using System;
using System.Numerics;
using System.Windows.Media;
using System.Threading.Tasks;

namespace CGA.Model
{
    public class PhongShading : FlatShading
    {
        public PhongShading(Obj obj, ColorBuffer buffer, Color color, PhongLighting lighting, Matrix4x4? modelMatrix, Vector3? emissionFactor)
            : base(obj, buffer, color, lighting)
        {
            if (Obj.NormalsTexture != null && modelMatrix == null)
            {
                throw new NullReferenceException(nameof(modelMatrix));
            }
            if (Obj.EmissionTexture != null && emissionFactor == null)
            {
                throw new NullReferenceException(nameof(emissionFactor));
            }
            ModelMatrix = modelMatrix;
            EmissionFactor = emissionFactor;
            IsTexturesEnabled = false; //Obj.NormalsTexture != null || Obj.DiffuseTexture != null ||
                                       // Obj.EmissionTexture != null || Obj.SpecularTexture != null;
        }

        public Vector3? EmissionFactor { get; }

        public Matrix4x4? ModelMatrix { get; }

        public bool IsTexturesEnabled { get; }
       
        public override void DrawModel()
        {
            ZBuffer.Reset();
            var faces = Obj
                .GetTriangleFaces()
                .Where(IsFaceVisible)
                .Select(face => (Face: face, Points: GetFaceColorPoints(face)));
            _ = Parallel.ForEach(faces, face =>
            {
                foreach (var p in face.Points)
                {
                    if (p.Z <= ZBuffer[p.X, p.Y])
                    {
                        ZBuffer[p.X, p.Y] = p.Z;

                        var color = IsTexturesEnabled ?
                            ((PhongLighting)Lighting).GetPointColor(Obj, p.Normal / p.NW, p.Texel / p.NW, ModelMatrix, EmissionFactor) :
                            Lighting.GetPointColor(p.Normal, GetFaceColor(face.Face, Color));
                        DrawPoint(p.X, p.Y, color);
                    }
                }
            });
        }

        private IEnumerable<Point> GetFaceColorPoints(IList<Vector3> face)
        {
            var points = GetFacePoints(face)
                .Where(point => IsValidPoint(point.X, point.Y, point.Z))
                .ToList();
            var pointsInFace = GetFaceIhnerPoints(points);
            points.AddRange(pointsInFace);
            return points;
        }

        private IEnumerable<Point> GetFaceIhnerPoints(IEnumerable<Point> sidePoints)
        {
            if (!sidePoints.Any())
                return Enumerable.Empty<Point>();

            var minY = sidePoints.Select(pixel => pixel.Y).Min();
            var maxY = sidePoints.Select(pixel => pixel.Y).Max();

            return Enumerable.Range(minY, maxY - minY)
                .SelectMany(y =>
                {
                    var points = sidePoints
                        .Where(point => point.Y == y)
                        .OrderBy(point => point.X)
                        .ToList();

                    if (!points.Any())
                        return Enumerable.Empty<Point>();

                    var p1 = points.First();
                    var p2 = points.Last();

                    var dx = (p2.X - p1.X);
                    var dz = (p2.Z - p1.Z) / Math.Abs(dx);
                    var dn = (p2.Normal - p1.Normal) / dx;
                    var dt = (p2.Texel - p1.Texel) / dx;
                    var dnw = (p2.NW - p1.NW) / dx;

                    return Enumerable
                        .Range(0, p2.X - p1.X)
                        .Select(i =>
                        {
                            var x = i + p1.X;
                            var z = i * dz + p1.Z;
                            var nw = i * dnw + p1.NW;
                            var n = i * dn + p1.Normal;
                            var t = i * dt + p1.Texel;
                            return new Point(x, y, z, nw, n, t);
                        });
                });
        }

        private new IEnumerable<Point> GetFacePoints(IList<Vector3> face)
        {
            var lastFaceIndex = face.Count - 1;
            return Enumerable
                .Range(0, lastFaceIndex)
                .Select(i => GetSidePoints(face, i, i + 1))
                .Append(GetSidePoints(face, 0, lastFaceIndex))
                .SelectMany(points => points);
        }

        private IEnumerable<Point> GetSidePoints(IList<Vector3> face, int index1, int index2)
        {
            var n1 = GetFaceNormal(face, index1);
            var n2 = GetFaceNormal(face, index2);

            var v1 = GetFacePoint(face, index1);
            var v2 = GetFacePoint(face, index2);

            Vector3 t1;
            Vector3 t2;
            if (IsTexturesEnabled)
            {
                t1 = GetFaceTexture(face, index1);
                t2 = GetFaceTexture(face, index2);

                n1 /= v1.W;
                t1 /= v1.W;
                n2 /= v2.W;
                t2 /= v2.W;
            }
            else
            {
                v1.W = 1;
                v2.W = 1;
                t1 = Vector3.Zero;
                t2 = Vector3.Zero;
            }

            var x1 = Convert.ToInt32(v1.X);
            var y1 = Convert.ToInt32(v1.Y);

            var x2 = Convert.ToInt32(v2.X);
            var y2 = Convert.ToInt32(v2.Y);

            var p1 = new Point(x1, y1, v1.Z, 1 / v1.W, n1, t1);
            var p2 = new Point(x2, y2, v2.Z, 1 / v2.W, n2, t2);

            return GetLinePoints(p1, p2);
        }

        private IEnumerable<Point> GetLinePoints(Point p1, Point p2)
        {
            var dx = Math.Abs(p2.X - p1.X);
            var dy = Math.Abs(p2.Y - p1.Y);
            var dz = Math.Abs(p2.Z - p1.Z);

            var signX = Math.Sign(p2.X - p1.X);
            var signY = Math.Sign(p2.Y - p1.Y);
            var signZ = Math.Sign(p2.Z - p1.Z);

            var x = p1.X;
            var y = p1.Y;
            var z = p1.Z;

            var deltaZ = dz / dy;

            var err = dx - dy;

            var normal = p1.Normal;
            var texel = p1.Texel;
            var nw = p1.NW;

            var isSameX = Math.Abs(p1.X - p2.X) < Math.Abs(p2.Y - p1.Y);
            var v = isSameX ? dy : dx;
            var dn = (p2.Normal - p2.Normal) / v;
            var dt = (p2.Texel - p1.Texel) / v;
            var dnw = (p2.NW - p1.NW) / v;

            while (x != p2.X || y != p2.Y)
            {
                yield return new Point(x, y, z, nw, normal, texel);

                int err2 = err * 2;
                if (err2 > -dy)
                {
                    err -= dy;
                    x += signX;
                    if (!isSameX)
                    {
                        normal += dn;
                        texel += dt;
                        nw += dnw;
                    }
                }
                if (err2 < dx)
                {
                    err += dx;
                    y += signY;
                    z += signZ * deltaZ;
                    if (isSameX)
                    {
                        normal += dn;
                        texel += dt;
                        nw += dnw;
                    }
                }
            }
            yield return p2;
        }
    }
}
