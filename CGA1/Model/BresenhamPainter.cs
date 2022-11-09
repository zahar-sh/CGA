using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CGA1.Model
{
    public class BresenhamPainter : IObjPainter
    {
        public void Paint(Obj obj, WriteableBitmap bitmap, Color color)
        {
            var bresenham = new Bresenham(obj, new WritableImage(bitmap), color);
            bresenham.DrawModel();
        }
    }
}
