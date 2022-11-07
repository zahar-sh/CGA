using System;
using System.Numerics;

namespace CGA1.Model
{
    public static class Matrices
    {
        private static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        private static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static Matrix4x4 TranslationMatrix(Vector3 translation)
        {
            return TranslationMatrix(translation.X, translation.Y, translation.Z);
        }

        public static Matrix4x4 TranslationMatrix(float x, float y, float z)
        {
            return new Matrix4x4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1);
        }

        public static Matrix4x4 ScaleMatrix(float scale)
        {
            return new Matrix4x4(
                scale, 0, 0, 0,
                0, scale, 0, 0,
                0, 0, scale, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 RotationXMatrix(float angle)
        {
            return new Matrix4x4(
                1, 0, 0, 0,
                0, Cos(angle), -Sin(angle), 0,
                0, Sin(angle), Cos(angle), 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 RotationYMatrix(float angle)
        {
            return new Matrix4x4(
                Cos(angle), 0, Sin(angle), 0,
                0, 1, 0, 0,
                -Sin(angle), 0, Cos(angle), 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 RotationZMatrix(float angle)
        {
            return new Matrix4x4(
                Cos(angle), -Sin(angle), 0, 0,
                Sin(angle), Cos(angle), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 RotationMatrix(Vector3 rotation)
        {
            return RotationMatrix(rotation.X, rotation.Y, rotation.Z);
        }

        public static Matrix4x4 RotationMatrix(float angleX, float angleY, float angleZ)
        {
            return RotationXMatrix(angleX) *
                RotationYMatrix(angleY) *
                RotationZMatrix(angleZ);
        }

        public static Matrix4x4 ModelMatrix(Vector3 translation, Vector3 rotation, float scale)
        {
            return TranslationMatrix(translation) *
                RotationMatrix(rotation) *
                ScaleMatrix(scale);
        }
    }
}
