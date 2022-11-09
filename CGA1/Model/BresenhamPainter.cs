using System.Windows.Media;

namespace CGA1.Model
{
    public class BresenhamPainter : IObjPainter
    {
        public void Paint(Obj obj, WritableImage image, Color color)
        {
            var bresenham = new Bresenham(obj, image, color);
            bresenham.DrawModel();
        }
    }
}
