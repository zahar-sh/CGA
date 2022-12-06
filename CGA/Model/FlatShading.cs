using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGA.Model
{
    public class FlatShading : Bresenham
    {
        public FlatShading(Obj obj, ColorBuffer buffer, Color color, ILighting lighting)
            : base(obj, buffer, color)
        {
            Lighting = lighting;
            ZBuffer = new ZBuffer(buffer.Width, buffer.Height);
        }

        public ILighting Lighting { get; set; }

        public ZBuffer ZBuffer { get; }

        public override void DrawModel()
        {
            ZBuffer.Reset();
            _ = Parallel.ForEach(Obj.GetTriangleFaces(), face =>
            {
                if (IsFaceVisible(face))
                {
                    var points = GetFacePoints(face);
                    var color = GetFaceColor(face, Color);
                    foreach (var p in points)
                    {
                        if (IsValidPoint(p.X, p.Y, p.Z))
                        {
                            if (p.Z <= ZBuffer[p.X, p.Y])
                            {
                                ZBuffer[p.X, p.Y] = p.Z;

                                DrawPoint(p.X, p.Y, color);
                            }
                        }
                    }
                }
            });
        }

        protected IEnumerable<Point> GetFacePoints(IList<Vector3> face)
        {
            var points = GetFaceSidePoints(face).ToList();
            var ihnerPoints = GetFaceIhnerPoints(points);
            points.AddRange(ihnerPoints);
            return points;
        }

        protected IEnumerable<Point> GetFaceIhnerPoints(IEnumerable<Point> sidePoints)
        {
            return sidePoints
                .GroupBy(p => p.Y)
                .SelectMany(g =>
                {
                    var y = g.Key;
                    var points = g.OrderBy(p => p.X).ToList();
                    return points.Count == 0 
                        ? Enumerable.Empty<Point>() 
                        : GetLinePoints(y, points.First(), points.Last());
                });
        }

        protected virtual IEnumerable<Point> GetLinePoints(int y, Point p1, Point p2)
        {
            var dx = Math.Abs(p2.X - p1.X);
            var dz = (p2.Z - p1.Z) / dx;

            return Enumerable
                .Range(p1.X, p2.X - p1.X)
                .Select(x =>
                {
                    var dx0 = (x - p1.X);
                    var z0 = dx0 * dz + p1.Z;
                    return new Point(x, y, z0);
                });
        }

        protected Color GetFaceColor(IList<Vector3> face, Color color)
        {
            var colors = face
                .Select(f => Convert.ToInt32(f.Z))
                .Select(index => Obj.Normals[index])
                .Select(normal => Lighting.GetColor(color, normal))
                .ToList();
            return AverageColor(colors);
        }

        protected Color AverageColor(IEnumerable<Color> colors)
        {
            var averageA = Convert.ToByte(colors.Select(color => (int)color.A).Average());
            var averageR = Convert.ToByte(colors.Select(color => (int)color.R).Average());
            var averageG = Convert.ToByte(colors.Select(color => (int)color.G).Average());
            var averageB = Convert.ToByte(colors.Select(color => (int)color.B).Average());
            return Color.FromArgb(averageA, averageR, averageG, averageB);
        }
    }
}
