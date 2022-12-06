using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public interface ILighting
    {
        Color GetColor(Color color, Vector3 normal);
    }
}