using System;

namespace CGA1.Model
{
    public class ZBuffer
    {
        private float[,] Data { get; }

        public int Width { get; }

        public int Height { get; }

        public ZBuffer(int width, int height)
        {
            Width=width;
            Height=height;
            Data = new float[Width, Height];
            Reset();
        }

        public void Reset()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[x, y] = float.MaxValue;
                }
            }
        }

        public bool IsValidIndexes(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public float this[int x, int y]
        {
            get
            {
                if (!IsValidIndexes(x, y))
                {
                    throw new ArgumentOutOfRangeException(nameof(x));
                }
                return Data[x, y];
            }

            set
            {
                if (!IsValidIndexes(x, y))
                {
                    throw new ArgumentOutOfRangeException(nameof(x));
                }
                Data[x, y] = value;
            }
        }
    }
}
