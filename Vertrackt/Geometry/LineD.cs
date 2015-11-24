using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public struct LineD : IScaleDown<LineD>
    {
        public override bool Equals(object obj)
        {
            if (!(obj is LineD))
            {
                return false;
            }

            return ((LineD)obj) == this;
        }

        public static bool operator ==(LineD a, LineD b)
        {
            return a.A == b.A && a.B == b.B;
        }

        public static bool operator !=(LineD a, LineD b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (A.GetHashCode() * 397) ^ B.GetHashCode();
        }

        public LineD ScaleDown(int scale)
        {
            return new LineD(A.ScaleDown(scale), B.ScaleDown(scale));
        }

        public LineD ScaleUp(int scale)
        {
            return new LineD(A.ScaleUp(scale), B.ScaleUp(scale));
        }


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

        public bool IsOnLine(PointD point, bool excludeStartAndEnd)
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


        public LineD ShiftOrthoToDirection(double distance)
        {
            var orthoVector = Direction.CrossProd(1);

            var shift = orthoVector.Normalize() * distance;

            return new LineD(A + shift, B + shift);
        }

        public static LineD FromPointAndDirection(PointD a, PointD direction)
        {
            return new LineD(a, a + direction);
        }

        public override string ToString()
        {
            return $"({A},{B})";
        }


        public static List<LineD> CreateLists(int[] points, int scale, int shift)
        {
            var allPoint = points.SlicteToChunks(2)
                .Select(item => new PointD(item.First(), item.Last())).ToList();

            if (allPoint.Count < 2)
            {
                return new List<LineD>();
            }

            var results = new List<LineD>();
            for (int i = 0; i < allPoint.Count() - 1; i++)
            {
                var a = allPoint[i];
                var b = allPoint[i + 1];

                if (a == b)
                {
                    continue;
                }

                var line = new LineD(a, b).ScaleUp(scale).ShiftOrthoToDirection(shift);
                results.Add(line);
            }

            for (int i = 0; i < results.Count() - 1; i++)
            {
                var a = results[i];
                var b = results[i + 1];

                if (a.B != b.A)
                {
                    var interSection = a.Intersection(b);

                    results[i] = new LineD(a.A, interSection);
                    results[i + 1] = new LineD(interSection, b.B);
                }
                else
                {
                    results[i] = new LineD(a.A, b.B);
                    results.RemoveAt(i + 1);
                }
            }

            return results;
        }
    }
}
