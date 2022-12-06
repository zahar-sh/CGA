using System;
using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public class LambertLighting : ILighting
    {
        public LambertLighting(Vector3 lighitngDirection)
        {
            LightingDirection = Vector3.Normalize(lighitngDirection);
        }

        public Vector3 LightingDirection { get; }

        public Color GetColor(Color color, Vector3 normal)
        {
            var v = Math.Max(Vector3.Dot(normal, LightingDirection), 0);
            var r = (byte)(color.R * v);
            var g = (byte)(color.G * v);
            var b = (byte)(color.B * v);
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}
