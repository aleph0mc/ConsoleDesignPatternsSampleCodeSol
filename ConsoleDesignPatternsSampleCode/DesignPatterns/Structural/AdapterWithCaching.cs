using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using static System.Console;

namespace ConsoleDesignPatternsSampleCode.DesignPatterns.Structural
{
    public class Point2
    {
        public int X;
        public int Y;

        public Point2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        protected bool Equals(Point2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class LineNew
    {
        public Point2 Start;
        public Point2 End;

        public LineNew(Point2 start, Point2 end)
        {
            this.Start = start;
            this.End = end;
        }

        protected bool Equals(LineNew other)
        {
            return Equals(Start, other.Start) && Equals(End, other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LineNew)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Start != null ? Start.GetHashCode() : 0) * 397) ^ (End != null ? End.GetHashCode() : 0);
            }
        }
    }

    public abstract class VectorObject2 : Collection<LineNew>
    {
    }

    public class VectorRectangle2 : VectorObject2
    {
        public VectorRectangle2(int x, int y, int width, int height)
        {
            Add(new LineNew(new Point2(x, y), new Point2(x + width, y)));
            Add(new LineNew(new Point2(x + width, y), new Point2(x + width, y + height)));
            Add(new LineNew(new Point2(x, y), new Point2(x, y + height)));
            Add(new LineNew(new Point2(x, y + height), new Point2(x + width, y + height)));
        }
    }

    public class LineToPointAdapter2 : IEnumerable<Point2>
    {
        private static int count = 0;
        static Dictionary<int, List<Point2>> cache = new Dictionary<int, List<Point2>>();
        private int hash;

        public LineToPointAdapter2(LineNew line)
        {
            hash = line.GetHashCode();
            if (cache.ContainsKey(hash)) return; // we already have it

            WriteLine($"{++count}: Generating points for line [{line.Start.X},{line.Start.Y}]-[{line.End.X},{line.End.Y}] (with caching)");
            //                                                 ^^^^

            List<Point2> points = new List<Point2>();

            int left = Math.Min(line.Start.X, line.End.X);
            int right = Math.Max(line.Start.X, line.End.X);
            int top = Math.Min(line.Start.Y, line.End.Y);
            int bottom = Math.Max(line.Start.Y, line.End.Y);
            int dx = right - left;
            int dy = line.End.Y - line.Start.Y;

            if (dx == 0)
            {
                for (int y = top; y <= bottom; ++y)
                {
                    points.Add(new Point2(left, y));
                }
            }
            else if (dy == 0)
            {
                for (int x = left; x <= right; ++x)
                {
                    points.Add(new Point2(x, top));
                }
            }

            cache.Add(hash, points);
        }

        public IEnumerator<Point2> GetEnumerator()
        {
            return cache[hash].GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class AdapterWithCaching
    {
        private static readonly List<VectorObject2> vectorObjects = new List<VectorObject2>
            {
              new VectorRectangle2(1, 1, 10, 10),
              new VectorRectangle2(3, 3, 6, 6)
            };

        // the interface we have
        public static void DrawPoint(Point2 p)
        {
            Write(".");
        }

        public static void CreateAdapterWithCaching()
        {
            Draw();
            Draw();
        }

        private static void Draw()
        {
            foreach (var vo in vectorObjects)
            {
                foreach (var line in vo)
                {
                    var adapter = new LineToPointAdapter2(line);
                    adapter.ForEach(DrawPoint);
                }
            }
        }
    }
}
