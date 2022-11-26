using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGA.Model
{
    public class ColorBuffer : Matrix<Color>
    {
        public ColorBuffer(int width, int height) : base(width, height)
        {
        }

        public void Fill(int startX, int startY, int endX, int endY, Color color)
        {
            SetElements(startX, startY, endX, endY, (x, y) => color);
        }

        public void Fill(Color color)
        {
            SetElements((x, y) => color);
        }

        public void FromBytes(byte[] bytes)
        {
            SetElements((x, y) =>
            {
                var index = y * Height + x * 4;
                var b = bytes[index];
                var g = bytes[index + 1];
                var r = bytes[index + 2];
                var a = bytes[index + 3];
                return Color.FromArgb(a, r, g, b);
            });
        }

        public byte[] ToBytes()
        {
            return Enumerable
                .Range(0, Height)
                .SelectMany(y => Enumerable
                    .Range(0, Width)
                    .Select(x => Data[x, y])
                    .SelectMany(color => GetBgraComponents(color)))
                .AsParallel()
                .AsOrdered()
                .ToArray();
        }

        public void Write(WriteableBitmap bitmap)
        {
            var bytes = ToBytes();
            bitmap.WritePixels(new Int32Rect(0, 0, Width, Height), bytes, bitmap.BackBufferStride, 0);
        }

        public static ColorBuffer From(BitmapSource image)
        {
            var bitmap = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgra32, null, 0));
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            var bytes = new byte[height * width * 4];
            bitmap.CopyPixels(bytes, bitmap.BackBufferStride, 0);

            var buffer = new ColorBuffer(width, height);
            buffer.FromBytes(bytes);
            return buffer;
        }

        private static IEnumerable<byte> GetBgraComponents(Color color)
        {
            yield return color.B;
            yield return color.G;
            yield return color.R;
            yield return color.A;
        }
    }
}
