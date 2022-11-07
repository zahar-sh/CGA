using System.Numerics;

namespace CGA1.Model
{
    public static class Matrices
    {
        public static Matrix4x4 CreateViewportMatrix(int minX, int minY, int width, int height)
        {
            return new Matrix4x4(
                width / 2, 0, 0, minX + (width / 2),
                0, -height / 2, 0, minY + (height / 2),
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

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

        public static Matrix4x4 GetWorldProjectionMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }


        public static Matrix4x4 CreateMatrix(Vector3 camera, Vector3 translation, float scale, float yaw, float pitch, float roll,
            float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            return CreateWorldMatrix(translation, scale, yaw, pitch, roll) *
                CreateViewerMatrix(camera, yaw, pitch, roll) *
                GetWorldProjectionMatrix(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }
    }
}
