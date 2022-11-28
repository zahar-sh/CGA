namespace CGA.Model
{
    public class ZBuffer 
    {
        public ZBuffer(int width, int height)
        {
            Data = new float[width, height];
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        private float[,] Data { get; }

        public float this[int x, int y]
        {
            get => Data[x, y];
            set => Data[x, y] = value;
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
    }
}