using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CGA1.Model
{
    public class ObjParser
    {
        public Obj Parse(TextReader reader)
        {
            var vertices = new List<Vector4>();
            var textures = new List<Vector3>();
            var normals = new List<Vector3>();
            var faces = new List<IList<Vector3>>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var strings = line.Split(' ');
                if (strings.Length == 0)
                    continue;
                var type = strings[0];
                var args = strings.Skip(1);
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
                            .Select(f => ParseFace(f.Split('/')) - Vector3.One)
                            .ToList();
                        faces.Add(face);
                        break;
                }
            }
            return new Obj(vertices, textures, normals, faces);
        }

        private Vector4 ParseVertex(IEnumerable<string> args)
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

        private Vector3 ParseTexture(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var y = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var z = e.MoveNext() ? ParseFloat(e.Current) : 0.0f;
                return new Vector3(x, y, z);
            }
        }

        private Vector3 ParseNormal(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var y = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                var z = e.MoveNext() ? ParseFloat(e.Current) : throw new Exception();
                return new Vector3(x, y, z);
            }
        }

        private Vector3 ParseFace(IEnumerable<string> args)
        {
            using (var e = args.GetEnumerator())
            {
                var x = e.MoveNext() ? int.Parse(e.Current) : throw new Exception();
                var y = e.MoveNext() && !string.IsNullOrEmpty(e.Current) ? int.Parse(e.Current) : 0;
                var z = e.MoveNext() && !string.IsNullOrEmpty(e.Current) ? int.Parse(e.Current) : 0;
                return new Vector3(x, y, z);
            }
        }

        private float ParseFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
