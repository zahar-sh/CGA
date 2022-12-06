using System;
using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public class PhongLighting : ILighting
    {
        public PhongLighting(Vector3 lighitngDirection, Vector3 viewerDirection, Vector3 ambientFactor, Vector3 diffuseFactor,
            Vector3 specularFactor, Vector3 diffuseColor, Vector3 specularColor, float shinessFactor,
            bool isNormalTextureEnabled, bool isDiffuseTextureEnabled, bool isSpecularTextureEnabled)
        {
            LightingDirection = Vector3.Normalize(lighitngDirection);
            ViewerDireciton = Vector3.Normalize(viewerDirection);
            AmbientFactor = ambientFactor;
            DiffuseFactor = diffuseFactor;
            SpecularFactor = specularFactor;
            DiffuseColor = diffuseColor;
            SpecularColor = specularColor;
            ShinessFactor = shinessFactor;
            IsNormalTextureEnabled= isNormalTextureEnabled;
            IsDiffuseTextureEnabled = isDiffuseTextureEnabled;
            IsSpecularTextureEnabled = isSpecularTextureEnabled;
            IsTexturesEnabled = isNormalTextureEnabled || isDiffuseTextureEnabled || isSpecularTextureEnabled;
        }

        public Vector3 LightingDirection { get; }
        public Vector3 ViewerDireciton { get; }
        public Vector3 AmbientFactor { get; }
        public Vector3 DiffuseFactor { get; }
        public Vector3 SpecularFactor { get; }
        public Vector3 DiffuseColor { get; }
        public Vector3 SpecularColor { get; }
        public float ShinessFactor { get; }

        public bool IsNormalTextureEnabled { get; }
        public bool IsDiffuseTextureEnabled { get; }
        public bool IsSpecularTextureEnabled { get; }
        public bool IsTexturesEnabled { get; }

        public Color GetColor(Color color, Vector3 normal)
        {
            var ambientLighting = Utils.AsVector(color) * AmbientFactor;
            var diffuseLighting = DiffuseColor * DiffuseFactor *
                Math.Max(Vector3.Dot(LightingDirection, normal), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(LightingDirection, normal));
            var specularLighting = SpecularColor * SpecularFactor *
                (float)Math.Pow(Math.Max(Vector3.Dot(ViewerDireciton, reflectionVector), 0), ShinessFactor);
            var resultLighting = (ambientLighting + diffuseLighting + specularLighting) / 255f;
            return Color.FromScRgb(1f, resultLighting.X, resultLighting.Y, resultLighting.Z);
        }

        public Color GetColor(Obj obj, Vector3 normal, Vector3 texel, Matrix4x4? modelMatrix)
        {
            if (!IsTexturesEnabled || obj is null)
                return Colors.Transparent;

            var texture = obj.NormalsTexture ?? obj.DiffuseTexture ?? obj.SpecularTexture;
            if (texture is null || Utils.IsInvalidVector(texel) || texel.X < 0 || (1 - texel.Y) < 0)
                return Colors.Transparent;

            var width = texture.Width;
            var height = texture.Height;
            var x = (int)(texel.X * width) % width;
            var y = (int)((1 - texel.Y) * height) % height;

            if (IsNormalTextureEnabled && !(obj.NormalsTexture is null))
            {
                normal = Vector3.Normalize(Utils.AsVector(obj.NormalsTexture[x, y]) * 2 - new Vector3(255));
                if (modelMatrix.HasValue)
                {
                    normal = Vector3.Normalize(Vector3.TransformNormal(normal, modelMatrix.Value));
                }
            }

            var diffuseColor = IsDiffuseTextureEnabled && !(obj.DiffuseTexture is null)
                ? Utils.AsVector(obj.DiffuseTexture[x, y])
                : (DiffuseColor * DiffuseFactor);

            var specularColor = IsSpecularTextureEnabled && !(obj.SpecularTexture is null)
                ? Utils.AsVector(obj.SpecularTexture[x, y])
                : (SpecularColor * SpecularFactor);

            var ambientLighting = diffuseColor * AmbientFactor;
            var diffuseLighting = diffuseColor * Math.Max(Vector3.Dot(LightingDirection, normal), 0);
            var reflectionVector = Vector3.Normalize(Vector3.Reflect(LightingDirection, normal));
            var specularLighting = specularColor * (float)Math.Pow(Math.Max(Vector3.Dot(ViewerDireciton, reflectionVector), 0), ShinessFactor);
            var resultLighting = (ambientLighting + diffuseLighting + specularLighting) / 255f;
            return Color.FromScRgb(1f, resultLighting.X, resultLighting.Y, resultLighting.Z);
        }
    }
}
