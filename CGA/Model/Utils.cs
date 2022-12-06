using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGA.Model
{
    public static class Utils
    {
        public static Obj LoadObj(string fileName)
        {
            var objTask = Task.Run(() => ParseObj(fileName));
            var directory = Path.GetDirectoryName(fileName);
            var normalsTextureTask = Task.Run(() => LoadTexture(Path.Combine(directory, "Normal.png")));
            var diffuseTextureTask = Task.Run(() => LoadTexture(Path.Combine(directory, "Diffuse.png")));
            var specularTextureTask = Task.Run(() => LoadTexture(Path.Combine(directory, "Specular.png")));

            var obj = objTask.GetAwaiter().GetResult();
            obj.NormalsTexture = normalsTextureTask.GetAwaiter().GetResult();
            obj.DiffuseTexture = diffuseTextureTask.GetAwaiter().GetResult();
            obj.SpecularTexture = specularTextureTask.GetAwaiter().GetResult();
            return obj;
        }

        public static Obj ParseObj(string fileName)
        {
            using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                return ObjParser.Parse(reader);
            }
        }

        public static ColorBuffer LoadTexture(string path)
        {
            try
            {
                var image = new BitmapImage(new Uri(path, UriKind.Relative))
                {
                    CreateOptions = BitmapCreateOptions.None
                };
                var bitmap = new WriteableBitmap(new FormatConvertedBitmap(new WriteableBitmap(image), PixelFormats.Bgra32, null, 0));
                var width = bitmap.PixelWidth;
                var height = bitmap.PixelHeight;
                var buffer = new ColorBuffer(width, height);
                buffer.Read(bitmap);
                return buffer;
            }
            catch
            {
                return null;
            }
        }

        public static WriteableBitmap CreateBitmap(int width, int height)
        {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        public static float ToRadians(float angle)
        {
            return (float)(angle / 180 * Math.PI);
        }

        public static bool IsInvalidVector(Vector3 vector)
        {
            return float.IsNaN(vector.X) || float.IsInfinity(vector.X) ||
                   float.IsNaN(vector.Y) || float.IsInfinity(vector.Y) ||
                   float.IsNaN(vector.Z) || float.IsInfinity(vector.Z);
        }

        public static Vector3 AsVector(Color color)
        {
            return new Vector3(color.R, color.G, color.B);
        }
    }
}
