using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CGA1.Model
{
    public interface IObjPainter
    {
        void Paint(Obj obj, WriteableBitmap bitmap, Color color);
    }
}
