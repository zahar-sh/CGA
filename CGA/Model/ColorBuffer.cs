using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGA.Model
{
    public class ColorBuffer
    {
        private static readonly int BytesPerPixel = 4;

        public ColorBuffer(int width, int height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }
            Width = width;
            Height = height;
            Data = new byte[Height * GetStride()];
        }

        public int Width { get; }

        public int Height { get; }

        private byte[] Data { get; }

        public Color this[int x, int y]
        {
            get
            {
                var offset = GetOffset(x, y);
                var b = Data[offset];
                var g = Data[offset + 1];
                var r = Data[offset + 2];
                var a = Data[offset + 3];
                return Color.FromArgb(a, r, g, b);
            }
            set
            {
                var offset = GetOffset(x, y);
                Data[offset] = value.B;
                Data[offset + 1] = value.G;
                Data[offset + 2] = value.R;
                Data[offset + 3] = value.A;
            }
        }

        private int GetStride()
        {
            return Width * BytesPerPixel;
        }

        private int GetOffset(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
            return y * GetStride() + x * BytesPerPixel;
        }

        public void Fill(Color color)
        {
            for (int offset = 0, end = Height * GetStride(); offset < end; offset += BytesPerPixel)
            {
                Data[offset] = color.B;
                Data[offset + 1] = color.G;
                Data[offset + 2] = color.R;
                Data[offset + 3] = color.A;
            }
        }

        public void Write(WriteableBitmap bitmap)
        {
            bitmap.WritePixels(new Int32Rect(0, 0, Width, Height), Data, GetStride(), 0);
        }

        public void Read(WriteableBitmap bitmap)
        {
            bitmap.CopyPixels(new Int32Rect(0, 0, Width, Height), Data, GetStride(), 0);
        }
    }
}