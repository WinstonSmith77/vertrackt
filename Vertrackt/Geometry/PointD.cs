using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public struct PointD : IScaleDown<PointD>
    {
        public PointD ScaleDown(int scale)
        {
           return new PointD(X / scale, Y /scale);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public bool Equals(PointD other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }

            return ((PointD)obj) == this;
        }

        public override int GetHashCode()
        {
            return ((int)X * 397) ^ (int)Y;
        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static PointD operator +(PointD a, PointD b)
        {
            return new PointD(a.X + b.X, a.Y + b.Y);
        }

        public static PointD operator -(PointD a, PointD b)
        {
            return new PointD(a.X - b.X, a.Y - b.Y);
        }


        public static bool operator ==(PointD a, PointD b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(PointD a, PointD b)
        {
            return !(a == b);
        }

        public double X { get; }

        public double Y { get; }

      
        public double Length => Math.Sqrt(X * X + Y * Y);

        public static PointD Zero
        {
            get { return _zero; }
        }

        private static readonly PointD _zero = new PointD(0, 0);


        public static PointD operator *(PointD a, double b)
        {
            return new PointD(a.X * b, a.Y * b);
        }

        public static implicit operator PointD(Point p)
        {
            return new PointD(p.X, p.Y);
        }

        public PointD Normalize()
        {
            var lenght = Length;

            return this / lenght;
        }

        public static PointD operator /(PointD a, double b)
        {
            return new PointD(a.X / b, a.Y / b);
        }

        public PointD CrossProd(double y)
        {
            return new PointD(Y * y, -X * y);
        }

        public static PointD operator -(PointD a)
        {
            return new PointD(-a.X, -a.Y);
        }
    }
}
