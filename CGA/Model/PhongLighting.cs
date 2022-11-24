using System;
using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public class PhongLighting : ILighting
    {
        public PhongLighting(Vector3 position, Vector3 direction, Vector3 backgroundFactor, Vector3 diffuseFactor,
            Vector3 mirrorFactor, Vector3 ambientColor, Vector3 reflectionColor, float shinessFactor)
        {
            Position = position;
            Direction = direction;
            BackgroundFactor = backgroundFactor;
            DiffuseFactor = diffuseFactor;
            MirrorFactor = mirrorFactor;
            AmbientColor = ambientColor;
            ReflectionColor = reflectionColor;
            ShinessFactor = shinessFactor;
        }

        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public Vector3 BackgroundFactor { get; }
        public Vector3 DiffuseFactor { get; }
        public Vector3 MirrorFactor { get; }
        public Vector3 AmbientColor { get; }
        public Vector3 ReflectionColor { get; }
        public float ShinessFactor { get; }

        public Color GetPointColor(Vector3 normal, Color color)
        {
            var backgroundLighting = BackgroundFactor * AmbientColor;
            var diffuseLighting = new Vector3(color.R, color.G, color.B) * DiffuseFactor *
                Math.Max(Vector3.Dot(normal, Vector3.Normalize(Position)), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(Position, normal));
            var mirrorLighting = ReflectionColor * MirrorFactor *
                (float)Math.Pow(Math.Max(Vector3.Dot(reflectionVector, Direction), 0), ShinessFactor);
            var resultLighting = (backgroundLighting + diffuseLighting + mirrorLighting) / 255f;
            return Color.FromScRgb(1f, resultLighting.X, resultLighting.Y, resultLighting.Z);
        }
    }
}
