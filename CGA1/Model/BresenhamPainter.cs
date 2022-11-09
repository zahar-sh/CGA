using System.Windows.Media.Imaging;

namespace CGA1.Model
{
    public class BresenhamPainter : IObjPainter
    {
        public void Paint(Obj obj, WriteableBitmap bitmap)
        {
            var bresenham = new Bresenham(obj, new WritableImage(bitmap));
            bresenham.DrawModel();
        }
    }
}
