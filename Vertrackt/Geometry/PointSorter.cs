using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class PointSorter : IComparer<Point>
    {
        private readonly double _angle;

        public PointSorter(double angle)
        {
            _angle = angle;
        }

        public int Compare(Point a, Point b)
        {
            var deltaA = Steps.DeltaAngle(_angle, a.Angle);
            var deltaB = Steps.DeltaAngle(_angle, b.Angle);

            var result = deltaA.CompareTo(deltaB);

            if (result != 0)
            {
                return result;
            }

            var lengthA = a.LengthSqr;
            var lenghtB = b.LengthSqr;

            return lenghtB.CompareTo(lengthA);
        }
    }
}
