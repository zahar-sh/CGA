using System;
using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public class LambertLighting : ILighting
    {
        public LambertLighting(Vector3 position)
        {
            Position = Vector3.Normalize(position);
        }

        public Vector3 Position { get; }

        public Color GetPointColor(Vector3 normal, Color color)
        {
            var v = Math.Max(Vector3.Dot(normal, Position), 0);
            var r = Convert.ToByte(color.R * v);
            var g = Convert.ToByte(color.G * v);
            var b = Convert.ToByte(color.B * v);
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}
