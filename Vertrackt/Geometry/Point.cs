using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public struct Point : IScaleDown<Point>
    {
        public Point ScaleDown(int scale)
        {
            return new Point(X / scale, Y / scale);
        }

        public override string ToString()
        {
            return $"{X};{Y}";
        }

        public string ToStringCt()
        {
            return $"({X},{Y})";
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }

            return ((Point)obj) == this;
        }

        public override int GetHashCode()
        {
            return (X * 397) ^ Y;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
            LengthSqr = x * x + y * y;
            Length = Math.Sqrt(LengthSqr);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }


        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }


        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public int X { get; }

        public int Y { get; }

        public double Angle => Math.Atan2(Y, X);

        public int LengthSqr
        {
            get; set;
        }

        public double Length
        {
            get; set;
        }

        public static Point Zero
        {
            get { return _zero; }
        }

        private static readonly Point _zero = new Point(0, 0);


        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y);
        }
    }
}
