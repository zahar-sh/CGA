using System;
using System.Numerics;

namespace CGA1.Model
{
    public static class Matrices
    {
        public static Matrix4x4 CreateTranslation(Vector3 v)
        {
            return CreateTranslation(v.X, v.Y, v.Z);
        }

        public static Matrix4x4 CreateTranslation(float x, float y, float z)
        {
            return new Matrix4x4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1);
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

        public static Matrix4x4 CreateRotation(Vector3 v)
        {
            return CreateRotation(v.X, v.Y, v.Z);
        }

        public static Matrix4x4 CreateRotation(float angleX, float angleY, float angleZ)
        {
            return CreateRotationX(angleX) *
                CreateRotationY(angleY) *
                CreateRotationZ(angleZ);
        }

        public static Matrix4x4 CreateModelMatrix(Vector3 translation, Vector3 rotation, float scale)
        {
            return CreateTranslation(translation) *
                CreateRotation(rotation) *
                CreateScale(scale);
        }

        public static Matrix4x4 CreateModelMatrix(float x, float y, float z, float angleX, float angleY, float angleZ, float scale)
        {
            return CreateTranslation(x, y, z) *
                CreateRotation(angleX, angleY, angleZ) *
                CreateScale(scale);
        }

        public static Matrix4x4 CreateViewMatrix(Vector3 eye, Vector3 rotation)
        {
            return CreateTranslation(-eye.X, -eye.Y, -eye.Z) *
                Matrix4x4.Transpose(CreateRotation(rotation.X, rotation.Y, rotation.Z));
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

        public static Matrix4x4 CreateProjectionByAspect(float aspect, float fov, float near, float far)
        {
            var v2 = (float)(1 / Math.Tan(fov / 2));
            var v1 = (float)((1 / aspect) * v2);
            var v3 = far / (near - far);
            var v4 = near * far / (near - far);
            return new Matrix4x4(
                v1, 0, 0, 0,
                0, v2, 0, 0,
                0, 0, v3, v4,
                0, 0, -1, 0);
        }

        public static Matrix4x4 CreateViewportMatrix(float minX, float minY, float width, float height)
        {
            var v1 = width / 2;
            var v2 = height / 2;
            return new Matrix4x4(
                v1, 0, 0, minX + v1,
                0, -v2, 0, minY + v2,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }
    }
}