using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public interface ILighting
    {
        Color GetPointColor(Vector3 normal, Color color);
    }
}