using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CGA1.Model
{
    public class WritableImage
    {
        private readonly int _width;
        private readonly int _height;
        private readonly long _backBuffer;
        private readonly int _backBufferStride;
        private readonly int _bytesPerPixel;

        public WritableImage(int width, int height)
        {
            Source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            _width = width;
            _height = height;
            _backBuffer = Source.BackBuffer.ToInt64();
            _backBufferStride = Source.BackBufferStride;
            _bytesPerPixel = Source.Format.BitsPerPixel / 8;
        }

        public WriteableBitmap Source { get; }

        public int Width => _width;

        public int Height => _height;

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
            return (byte*)(_backBuffer + (y * _backBufferStride) + (x * _bytesPerPixel));
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
