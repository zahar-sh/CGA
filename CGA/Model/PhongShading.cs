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
        public PhongShading(Obj obj, ColorBuffer buffer, Color color, ILighting lighting, Matrix4x4? modelMatrix)
            : base(obj, buffer, color, lighting)
        {
            ModelMatrix = modelMatrix;
        }

        public Matrix4x4? ModelMatrix { get; }

        public override void DrawModel()
        {
            ZBuffer.Reset();
            _ = Parallel.ForEach(Obj.GetTriangleFaces(), face =>
            {
                if (IsFaceVisible(face))
                {
                    var points = GetFacePoints(face);
                    foreach (var p in points)
                    {
                        if (IsValidPoint(p.X, p.Y, p.Z))
                        {
                            if (p.Z <= ZBuffer[p.X, p.Y])
                            {
                                ZBuffer[p.X, p.Y] = p.Z;

                                var color = Lighting is PhongLighting l && l.IsTexturesEnabled
                                        ? l.GetColor(Obj, p.Normal / p.NW, p.Texel / p.NW, ModelMatrix)
                                        : Lighting.GetColor(Color, p.Normal);
                                DrawPoint(p.X, p.Y, color);
                            }
                        }
                    }
                }
            });
        }

        protected override IEnumerable<Point> GetLinePoints(int y, Point p1, Point p2)
        {
            var dx = Math.Abs(p2.X - p1.X);
            var dz = (p2.Z - p1.Z) / dx;
            var dnw = (p2.NW - p1.NW) / dx;
            var dn = (p2.Normal - p1.Normal) / dx;
            var dt = (p2.Texel - p1.Texel) / dx;

            return Enumerable
                .Range(p1.X, p2.X - p1.X)
                .Select(x =>
                {
                    var dv = x - p1.X;
                    var z = dz * dv + p1.Z;
                    var nw = dnw * dv + p1.NW;
                    var n = dn * dv + p1.Normal;
                    var t = dt * dv + p1.Texel;
                    return new Point(x, y, z, nw, n, t);
                });
        }

        protected override Point GetPoint(IList<Vector3> face, int i)
        {
            var v = GetFacePoint(face, i);
            var x = Convert.ToInt32(v.X);
            var y = Convert.ToInt32(v.Y);
            var nw = 1.0f / v.W;
            var n = GetFaceNormal(face, i) * nw;
            var t = GetFaceTexture(face, i) * nw;
            return new Point(x, y, v.Z, nw, n, t);
        }

    }
}
