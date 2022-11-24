using System;
using System.Numerics;

namespace CGA.Model
{
    public static class Matrices
    {
        public static Matrix4x4 CreateTranslation(float x, float y, float z)
        {
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                x, y, z, 1);
        }

        public static Matrix4x4 CreateScale(float scale)
        {
            return new Matrix4x4(
                scale, 0, 0, 0,
                0, scale, 0, 0,
                0, 0, scale, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 CreateRotationX(float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);
            return new Matrix4x4(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 CreateRotationY(float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);
            return new Matrix4x4(
                cos, 0, sin, 0,
                0, 1, 0, 0,
                -sin, 0, cos, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 CreateRotationZ(float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);
            return new Matrix4x4(
                cos, -sin, 0, 0,
                sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 CreateRotation(float angleX, float angleY, float angleZ)
        {
            /*return CreateRotationX(angleX) *
                CreateRotationY(angleY) *
                CreateRotationZ(angleZ);*/
            return Matrix4x4.CreateFromYawPitchRoll(angleX, angleY, angleZ);
        }

        public static Matrix4x4 CreateModelMatrix(float x, float y, float z, float angleX, float angleY, float angleZ, float scale)
        {
            return CreateScale(scale) *
                CreateRotation(angleX, angleY, angleZ) *
                 CreateTranslation(x, y, z);
        }

        public static Matrix4x4 CreateViewMatrix(float x, float y, float z, float angleX, float angleY, float angleZ)
        {
            return CreateTranslation(-x, -y, -z) *
                Matrix4x4.Transpose(CreateRotation(angleX, angleY, angleZ));
        }

        public static Matrix4x4 CreateProjectionBySize(float width, float height, float near, float far)
        {
            var v1 = 2 * near / width;
            var v2 = 2 * near / height;
            var v3 = far / (near - far);
            var v4 = near * far / (near - far);
            return new Matrix4x4(
                v1, 0, 0, 0,
                0, v2, 0, 0,
                0, 0, v3, v4,
                0, 0, -1, 0);
        }

        public static Matrix4x4 CreateProjectionByAspect(float fov, float aspect, float near, float far)
        {
            /*var v2 = (float)(1 / Math.Tan(fov / 2));
            var v1 = (float)((1 / aspect) * v2);
            var v3 = far / (near - far);
            var v4 = near * far / (near - far);
            return new Matrix4x4(
                v1, 0, 0, 0,
                0, v2, 0, 0,
                0, 0, v3, v4,
                0, 0, -1, 0);*/
            return Matrix4x4.CreatePerspectiveFieldOfView(fov, aspect, near, far);
        }

        public static Matrix4x4 CreateViewportMatrix(float minX, float minY, float width, float height)
        {
            var v1 = width / 2;
            var v2 = height / 2;
            return new Matrix4x4(
                v1, 0, 0, 0,
                0, -v2, 0, 0,
                0, 0, 1, 0,
                minX + v1, minY + v2, 0, 1);
        }
    }
}