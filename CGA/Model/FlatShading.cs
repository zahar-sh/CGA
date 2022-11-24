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
            _ = Parallel.ForEach(Obj.GetTriangleFaces(), face =>
            {
                var f = face.ToList();
                if (IsFaceVisible(f))
                {
                    var color = GetFaceColor(f, Color);
                    DrawFace(f, color);
                }
            });
        }

        protected override void DrawPoint(int x, int y, float z, Color color)
        {
            if (x >= 0 && x < Buffer.Width &&
                y >= 0 && y < Buffer.Height &&
                z > 0 && z < 1 &&
                z <= ZBuffer[x, y])
            {
                Buffer[x, y] = color;
                ZBuffer[x, y] = z;
            }
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
    }
}
