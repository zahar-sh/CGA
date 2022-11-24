using System.Numerics;
using System.Windows.Media;

namespace CGA.Model
{
    public struct Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Z { get; set; }
        public float NW { get; set; }
        public Color Color { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Texel { get; set; }

        public Pixel(int x, int y, float z, Color color)
        {
            X = x;
            Y = y;
            Z = z;
            NW = 1.0f;
            Color = color;
            Normal = new Vector3(0);
            Texel = new Vector3(0);
        }

        public Pixel(int x, int y, float z, float nw, Color color, Vector3 normal, Vector3 texel)
        {
            X = x;
            Y = y;
            Z = z;
            NW = nw;
            Color = color;
            Normal = normal;
            Texel = texel;
        }
    }
}
