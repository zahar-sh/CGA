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

        public Color GetPointColor(Obj obj, Vector3 normal, Vector3 texel, Matrix4x4? modelMatrix, Vector3? emissionFactor)
        {
            if (obj.DiffuseTexture is null ||
                IsInvalidVector(normal) ||
                IsInvalidVector(texel) ||
                texel.X < 0 ||
                (1 - texel.Y) < 0)
            {
                return Colors.Transparent;
            }

            var x = (int)(texel.X * obj.DiffuseTexture.Width);
            var y = (int)((1 - texel.Y) * obj.DiffuseTexture.Height);
            x %= obj.DiffuseTexture.Width;
            y %= obj.DiffuseTexture.Height;

            if (obj.NormalsTexture != null && modelMatrix.HasValue)
            {
                var v = From(obj.NormalsTexture[x, y]) * 2 - new Vector3(255);
                normal = Vector3.Normalize(Vector3.TransformNormal(Vector3.Normalize(v), modelMatrix.Value));
            }

            var backgroundLighting = From(obj.DiffuseTexture[x, y]) * AmbientFactor;
            var diffuseLighting = From(obj.DiffuseTexture[x, y]) * Math.Max(Vector3.Dot(normal, Position), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(Position, normal));
            var mirrorLighting = obj.SpecularTexture is null ? Vector3.Zero :
                (From(obj.SpecularTexture[x, y]) * (float)Math.Pow(Math.Max(0, Vector3.Dot(Direction, reflectionVector)), ShinessFactor));
            var emissionLighting = obj.EmissionTexture is null && emissionFactor.HasValue ? Vector3.Zero :
                           (From(obj.EmissionTexture[x, y]) * emissionFactor.Value);
            var resultLighting = (backgroundLighting + diffuseLighting + mirrorLighting + emissionLighting) / 255f;
            return Color.FromScRgb(1f, resultLighting.X, resultLighting.Y, resultLighting.Z);
        }

        private static bool IsInvalidVector(Vector3 vector)
        {
            return float.IsNaN(vector.X) || float.IsInfinity(vector.X) ||
                   float.IsNaN(vector.Y) || float.IsInfinity(vector.Y) ||
                   float.IsNaN(vector.Z) || float.IsInfinity(vector.Z);
        }

        private static Vector3 From(Color color)
        {
            return new Vector3(color.R, color.G, color.B);
        }
    }
}
