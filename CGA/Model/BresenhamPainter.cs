using System.Windows.Media;

namespace CGA1.Model
{
    public class BresenhamPainter : IObjPainter
    {
        public void Paint(Obj obj, ColorBuffer buffer, Color color)
        {
            var bresenham = new Bresenham(obj, buffer, color);
            bresenham.DrawModel();
        }
    }
}