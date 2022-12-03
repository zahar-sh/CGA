using System;
using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public class PhongLighting : ILighting
    {
        public PhongLighting(Vector3 position, Vector3 direction, Vector3 ambientFactor, Vector3 diffuseFactor,
            Vector3 specularFactor, Vector3 ambientColor, Vector3 specularColor, float shinessFactor)
        {
            Position = Vector3.Normalize(position);
            Direction = Vector3.Normalize(direction);
            AmbientFactor = ambientFactor;
            DiffuseFactor = diffuseFactor;
            SpecularFactor = specularFactor;
            AmbientColor = ambientColor;
            SpecularColor = specularColor;
            ShinessFactor = shinessFactor;
        }

        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public Vector3 AmbientFactor { get; }
        public Vector3 DiffuseFactor { get; }
        public Vector3 SpecularFactor { get; }
        public Vector3 AmbientColor { get; }
        public Vector3 SpecularColor { get; }
        public float ShinessFactor { get; }

        public Color GetPointColor(Vector3 normal, Color color)
        {
            var backgroundLighting = AmbientColor * AmbientFactor;
            var diffuseLighting = new Vector3(color.R, color.G, color.B) * DiffuseFactor *
                Math.Max(Vector3.Dot(normal, Position), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(Position, normal));
            var specularLighting = SpecularColor * SpecularFactor *
                (float)Math.Pow(Math.Max(Vector3.Dot(reflectionVector, Direction), 0), ShinessFactor);
            var resultLighting = (backgroundLighting + diffuseLighting + specularLighting) / 255f;
            return Color.FromScRgb(1f, resultLighting.X, resultLighting.Y, resultLighting.Z);
        }
    }
}
