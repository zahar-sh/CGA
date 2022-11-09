using System.Windows.Media.Imaging;

namespace CGA1.Model
{
    public interface IObjPainter
    {
        void Paint(Obj obj, WriteableBitmap bitmap);
    }
}
