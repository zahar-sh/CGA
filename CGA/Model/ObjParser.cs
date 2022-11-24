using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CGA.Model
{
    public static class ObjParser
    {
        private static readonly char[] WhiteSpaceSeparator = new char[] { ' ' };
        private static readonly char[] FaceSeparator = new char[] { ' ', '/' };

        public static Obj Parse(TextReader reader)
        {
            var vertices = new List<Vector4>();
            var textures = new List<Vector3>();
            var normals = new List<Vector3>();
            var faces = new List<IList<Vector3>>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var strings = Split(line, WhiteSpaceSeparator);
                if (strings.Length == 0)
                    continue;
                var type = strings[0];
                var args = strings.Skip(1);
                try
                {
                    switch (type)
                    {
                        case "v":
                            var vertex = ParseVertex(args);
                            vertices.Add(vertex);
                            break;
                        case "vt":
                            var texture = ParseTexture(args);
                            textures.Add(texture);
                            break;
                        case "vn":
                            var normal = ParseNormal(args);
                            normals.Add(normal);
                            break;
                        case "f":
                            var face = args
                                .Select(f => ParseFace(Split(f, FaceSeparator)) - Vector3.One)
                                .ToList();
                            faces.Add(face);
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(line, e);
                }
            }
            return new Obj()
            {
                Vertices = vertices,
                Textures = textures,
                Normals = normals,
                Faces = faces
            };
        }

        private static string[] Split(string s, char[] separator)
        {
            return s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Vector4 ParseVertex(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var y = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var z = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var w = e.MoveNext() ? ParseFloat(e.Current) : 1.0f;
                return new Vector4(x, y, z, w);
            }
        }

        private static Vector3 ParseTexture(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var y = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var z = e.MoveNext() ? ParseFloat(e.Current) : 0.0f;
                return new Vector3(x, y, z);
            }
        }

        private static Vector3 ParseNormal(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var y = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var z = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                return new Vector3(x, y, z);
            }
        }

        private static Vector3 ParseFace(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? int.Parse(e.Current) : throw new Exception();
                var y = e.MoveNext() && !string.IsNullOrEmpty(e.Current) ? int.Parse(e.Current) : 0;
                var z = e.MoveNext() && !string.IsNullOrEmpty(e.Current) ? int.Parse(e.Current) : 0;
                return new Vector3(x, y, z);
            }
        }

        private static float ParseFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
