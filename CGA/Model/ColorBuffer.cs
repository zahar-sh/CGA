using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGA1.Model
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
                .Select(y => Task.Run(() =>
                    Enumerable
                    .Range(0, Width)
                    .Select(x => Data[x, y])
                    .SelectMany(color => GetColorComponents(color))))
                .Select(task => task.GetAwaiter().GetResult())
                .SelectMany(line => line)
                .ToArray();
            bitmap.WritePixels(new Int32Rect(0, 0, Width, Height), bytes, bitmap.BackBufferStride, 0);
        }

        private IEnumerable<byte> GetColorComponents(Color color)
        {
            yield return color.B;
            yield return color.G;
            yield return color.R;
            yield return color.A;
        }
    }
}
