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

        public void Write(WriteableBitmap bitmap)
        {
            var bytes = Enumerable
                .Range(0, Height)
                .SelectMany(y => Enumerable
                    .Range(0, Width)
                    .Select(x => Data[x, y])
                    .SelectMany(color => GetBgraComponents(color)))
                .AsParallel()
                .AsOrdered()
                .ToArray();
            bitmap.WritePixels(new Int32Rect(0, 0, Width, Height), bytes, bitmap.BackBufferStride, 0);
        }

        private IEnumerable<byte> GetBgraComponents(Color color)
        {
            yield return color.B;
            yield return color.G;
            yield return color.R;
            yield return color.A;
        }
    }
}
