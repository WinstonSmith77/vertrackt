using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public struct LineD
    {
        public PointD A { get; }
        public PointD B { get; }
        public PointD Direction { get; }

        public LineD(PointD a, PointD b)
        {
            A = a;
            B = b;
            Direction = (B - A).Normalize();
        }

        public PointD Intersection(LineD otherLine)
        {
            var m = new[,]
                        {
                            {Direction.X, -otherLine.Direction.X, -A.X + otherLine.A.X},
                            {Direction.Y, -otherLine.Direction.Y, -A.Y + otherLine.A.Y}
                        };

            double[] solution = GaussSolver.Solve(m);

            PointD result = A + Direction * (float)solution[0];

            return result;
        }

        public PointD? IntersectionAndOnBothLines(LineD otherLine, bool excludeStartAndEnd)
        {
            var intersection = Intersection(otherLine);

            if (double.IsNaN(intersection.X) || double.IsNaN(intersection.Y))
            {
                return null;
            }

            bool thisIsOnline = IsOnLine(intersection, excludeStartAndEnd);
            bool otherIsOnline = otherLine.IsOnLine(intersection, excludeStartAndEnd);

            if (thisIsOnline && otherIsOnline)
            {
                return intersection;
            }
            return null;
        }

        private bool IsOnLine(PointD point, bool excludeStartAndEnd)
        {
            var distA = (point - A).Length;
            if (excludeStartAndEnd)
            {
                var onA = distA.IsApproxEqual(0);
                if (onA)
                {
                    return false;
                }
            }

            var distB = (point - B).Length;
            if (excludeStartAndEnd)
            {
                var onB = distB.IsApproxEqual(0);
                if (onB)
                {
                    return false;
                }
            }

            var length = (A - B).Length;
            return (distA + distB).IsApproxEqual(length);
        }

        private double Distance(PointD point)
        {
            double tempX = point.X - A.X;
            double tempY = point.Y - A.Y;

            double result = tempX * Direction.Y - tempY * Direction.X;

            return result;
        }

        public LineD LineDOrthoLineThrough(PointD point)
        {
            var orthoVector = Direction.CrossProd(1);


            if (Distance(point) < 0)
            {
                orthoVector = -orthoVector;
            }

            return FromPointAndDirection(point, orthoVector);
        }

        public static LineD FromPointAndDirection(PointD a, PointD direction)
        {
            return new LineD(a, a + direction);
        }

        public override string ToString()
        {
            return $"({A},{B})";
        }
    }
}
