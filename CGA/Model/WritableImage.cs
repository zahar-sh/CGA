using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CGA1.Model
{
    public class WritableImage
    {
        public WriteableBitmap Source { get; private set; }

        private long BackBuffer { get => Source.BackBuffer.ToInt64(); }

        private int BackBufferStride { get => Source.BackBufferStride; }

        private int BytesPerPixel { get => Source.Format.BitsPerPixel / 8; }

        public int Width { get => Source.PixelWidth; }

        public int Height { get => Source.PixelHeight; }

        public WritableImage(int width, int height)
        {
            Source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        public bool IsValidIndexes(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private unsafe byte* GetAddress(int x, int y)
        {
            if (!IsValidIndexes(x, y))
            {
                throw new IndexOutOfRangeException();
            }
            return (byte*)(BackBuffer + (y * BackBufferStride) + (x * BytesPerPixel));
        }

        public unsafe Color this[int x, int y]
        {
            get
            {
                byte* address = GetAddress(x, y);
                var a = address[3];
                var r = address[2];
                var g = address[1];
                var b = address[0];
                return Color.FromArgb(a, r, g, b);
            }
            set
            {
                byte* address = GetAddress(x, y);
                address[3] = value.A;
                address[2] = value.R;
                address[1] = value.G;
                address[0] = value.B;
            }
        }
    }
}
