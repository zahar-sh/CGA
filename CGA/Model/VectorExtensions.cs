﻿using System.Numerics;

namespace CGA.Model
{
    public static class Vectors
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
