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

        public ILighting Lighting { get; }

        public ZBuffer ZBuffer { get; }

        public override void DrawModel()
        {
            ZBuffer.Reset();
            var faces = Obj.GetTriangleFaces()
                .Select(face => face.ToList())
                .Where(IsFaceVisible)
                .Select(face =>
                {
                    var color = GetFaceColor(face, Color);
                    var points = GetFacePoints(face)
                        .Where(point => IsValidPoint(point.X, point.Y, point.Z));
                    return (Color: color, Points: points);
                });
            _ = Parallel.ForEach(faces, face =>
            {
                var points = face.Points.ToList();
                var pointsInFace = GetFaceIhnerPoints(points);

                lock (ZBuffer)
                {
                    foreach (var (X, Y, Z) in points.Concat(pointsInFace))
                    {
                        if (Z <= ZBuffer[X, Y])
                        {
                            ZBuffer[X, Y] = Z;
                            DrawPoint(X, Y, face.Color);
                        }
                    }
                }
            });
        }

        private Color GetFaceColor(IList<Vector3> face, Color color)
        {
            var colors = face
                .Select(f => Convert.ToInt32(f.Z))
                .Select(index => Obj.Normals[index])
                .Select(normal => Lighting.GetPointColor(normal, color))
                .ToList();
            return AverageColor(colors);
        }

        private Color AverageColor(IEnumerable<Color> colors)
        {
            var averageA = Convert.ToByte(colors.Select(color => (int)color.A).Average());
            var averageR = Convert.ToByte(colors.Select(color => (int)color.R).Average());
            var averageG = Convert.ToByte(colors.Select(color => (int)color.G).Average());
            var averageB = Convert.ToByte(colors.Select(color => (int)color.B).Average());
            return Color.FromArgb(averageA, averageR, averageG, averageB);
        }

        private IEnumerable<(int X, int Y, float Z)> GetFaceIhnerPoints(IEnumerable<(int X, int Y, float Z)> sidePoints)
        {
            if (!sidePoints.Any())
                return Enumerable.Empty<(int X, int Y, float Z)>();

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
                        return Enumerable.Empty<(int X, int Y, float Z)>();

                    var (X1, Y1, Z1) = points.First();
                    var (X2, Y2, Z2) = points.Last();

                    int dx = Math.Abs(X2 - X1);
                    float dz = Z2 - Z1;
                    float deltaZ = dz / dx;

                    return Enumerable
                        .Range(X1, X2 - X1)
                        .Select(x => (X: x, Y: y, Z: x * deltaZ));
                });
        }
    }
}
