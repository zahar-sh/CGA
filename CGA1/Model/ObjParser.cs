using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CGA1.Model
{
    public static class ObjParsers
    {
        private static bool IsPresent(string s) => !string.IsNullOrEmpty(s);

        private static float ParseFloat(string s) => float.Parse(s, CultureInfo.InvariantCulture);

        private static R NextOrElse<T, R>(IEnumerator<T> e, Predicate<T> predicate, Func<T, R> mapper, R value = default)
        {
            return e.MoveNext() && predicate(e.Current) ? mapper(e.Current) : value;
        }

        private static Vector3 ParseVector3(IEnumerable<string> values, Vector3 defaultValue = default)
        {

            using (var e = values.GetEnumerator())
            {
                var x = NextOrElse(e, IsPresent, ParseFloat, defaultValue.X);
                var y = NextOrElse(e, IsPresent, ParseFloat, defaultValue.Y);
                var z = NextOrElse(e, IsPresent, ParseFloat, defaultValue.Z);
                return new Vector3(x, y, z);
            }
        }

        private static Vector4 ParseVector4(IEnumerable<string> values, Vector4 defaultValue = default)
        {

            using (var e = values.GetEnumerator())
            {
                var x = NextOrElse(e, IsPresent, ParseFloat, defaultValue.X);
                var y = NextOrElse(e, IsPresent, ParseFloat, defaultValue.Y);
                var z = NextOrElse(e, IsPresent, ParseFloat, defaultValue.Z);
                var w = NextOrElse(e, IsPresent, ParseFloat, defaultValue.W);
                return new Vector4(x, y, z, w);
            }
        }

        public static Obj Parse(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("File doesn't exists");
            }
            var points = new ConcurrentQueue<Vector4>();
            var textures = new ConcurrentQueue<Vector3>();
            var normals = new ConcurrentQueue<Vector3>();
            var faces = new ConcurrentQueue<IList<Vector3>>();

            var readLinesBlock = new TransformManyBlock<string, string>(File.ReadLines);
            var splitLineBlock = new TransformBlock<string, string[]>(line => line.Split(' '));
            var pointParser = new ActionBlock<string[]>(strings =>
            {
                var args = strings.Skip(1);
                var point = ParseVector4(args, new Vector4(0, 0, 0, 1));
                points.Enqueue(point);
            });
            var textureParser = new ActionBlock<string[]>(strings =>
            {
                var args = strings.Skip(1);
                var texture = ParseVector3(args);
                textures.Enqueue(texture);
            });
            var normalParser = new ActionBlock<string[]>(strings =>
            {
                var args = strings.Skip(1);
                var normal = ParseVector3(args);
                textures.Enqueue(normal);
            });
            var facesParser = new ActionBlock<string[]>(strings =>
            {
                var args = strings.Skip(1);
                var face = args.Select(f => ParseVector3(f.Split('/'))).ToList();
                faces.Enqueue(face);
            });

            readLinesBlock.LinkTo(splitLineBlock);
            splitLineBlock.LinkTo(pointParser, strigs => "v".Equals(strigs[0]));
            splitLineBlock.LinkTo(textureParser, strigs => "vt".Equals(strigs[0]));
            splitLineBlock.LinkTo(normalParser, strigs => "vn".Equals(strigs[0]));
            splitLineBlock.LinkTo(facesParser, strigs => "f".Equals(strigs[0]));

            readLinesBlock.Post(path);

            var completion = Task.WhenAll(pointParser.Completion,
                textureParser.Completion,
                normalParser.Completion,
                facesParser.Completion);
            completion.GetAwaiter().GetResult();

            return new Obj(points.ToList(), textures.ToList(), normals.ToList(), faces.ToList());
        }
    }
}
