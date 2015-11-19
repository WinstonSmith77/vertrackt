using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public struct LineD
    {
        public PointD A  { get; }
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

        public double Distance(PointD point)
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
    }
}
