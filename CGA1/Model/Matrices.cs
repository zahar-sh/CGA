using System.Numerics;

namespace CGA1.Model
{
    public static class Matrices
    {
        public static Matrix4x4 CreateWorldMatrix(Vector3 translation, float scale, float yaw, float pitch, float roll)
        {
            return Matrix4x4.CreateTranslation(translation) *
                Matrix4x4.CreateScale(scale) *
                Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        public static Matrix4x4 CreateViewerMatrix(Vector3 camera, float yaw, float pitch, float roll)
        {
            return Matrix4x4.CreateTranslation(-camera) *
                Matrix4x4.Transpose(Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll));
        }

        public static Matrix4x4 CreateWorldProjectionMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public static Matrix4x4 CreateViewportMatrix(int minX, int minY, int width, int height)
        {
            var num = width / 2;
            var num2 = height / 2;
            return new Matrix4x4(
                num, 0, 0, minX + num,
                0, -num2, 0, minY + num2,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static void Transform(Obj obj, Matrix4x4 worldMatrix, Matrix4x4 cameraMatrix, Matrix4x4 worldProjectionMatrix, Matrix4x4 viewportMatrix)
        {
            var matrix = worldMatrix * cameraMatrix * worldProjectionMatrix;
            var vertices = obj.Vertices;
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i];
                var v1 = Vector4.Transform(Vector4.Transform(v, matrix) / v.W, viewportMatrix);
                v1.W = v.W;
                vertices[i] = v1;
            }
            var normals = obj.Normals;
            for (int i = 0; i < normals.Count; i++)
            {
                normals[i] = Vector3.Normalize(Vector3.TransformNormal(normals[i], worldMatrix));
            }
        }
    }
}
