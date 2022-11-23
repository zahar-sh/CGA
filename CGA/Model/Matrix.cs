using System;
using System.Collections.Generic;
using System.Linq;

namespace CGA1.Model
{
    public class Matrix<T>
    {
        public Matrix(int width, int height)
        {
            Data = new T[width, height];
            Width = width;
            Height = height;
        }

        protected T[,] Data { get; }

        public int Width { get; }

        public int Height { get; }

        public T this[int x, int y]
        {
            get
            {
                CheckX(x, nameof(x));
                CheckY(y, nameof(y));
                return Data[x, y];
            }

            set
            {
                CheckX(x, nameof(x));
                CheckY(y, nameof(y));
                Data[x, y] = value;
            }
        }

        private void CheckY(int y, string paramName)
        {
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        private void CheckX(int x, string paramName)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        public IEnumerable<(int x, int y)> GetIndexes(int startX, int startY, int endX, int endY)
        {
            CheckX(startX, nameof(startX));
            CheckY(startY, nameof(startY));
            CheckX(endX, nameof(endX));
            CheckY(endY, nameof(endY));
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public IEnumerable<(int x, int y)> GetIndexes()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public IEnumerable<T> GetElements(int startX, int startY, int endX, int endY)
        {
            return GetElements(GetIndexes(startX, startY, endX, endY));
        }

        public IEnumerable<T> GetElements()
        {
            return GetElements(GetIndexes());
        }

        public void SetElements(int startX, int startY, int endX, int endY, Func<int, int, T> selector)
        {
            SetElements(GetIndexes(startX, startY, endX, endY), selector);
        }

        public void SetElements(Func<int, int, T> selector)
        {
            SetElements(GetIndexes(), selector);
        }

        private IEnumerable<T> GetElements(IEnumerable<(int x, int y)> points)
        {
            return points.Select(point => Data[point.x, point.y]);
        }

        private void SetElements(IEnumerable<(int x, int y)> points, Func<int, int, T> selector)
        {
            foreach (var (x, y) in points)
            {
                Data[x, y] = selector(x, y);
            }
        }
    }
}
