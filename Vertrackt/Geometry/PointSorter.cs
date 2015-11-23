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
       // private readonly bool _hasSichtLinie;

        public PointSorter(double angle)
        {
            _angle = angle;
            //_hasSichtLinie = hasSichtLinie;
        }

        public int Compare(Point a, Point b)
        {
           /* if (_hasSichtLinie)
            {
                var resultAngel = CompareAngle(a, b);

                if (resultAngel != 0)
                {
                    return resultAngel;
                }
                return ComapreLength(a, b);
            }*/

            var resultLength = ComapreLength(a, b);

            if (resultLength != 0)
            {
                return resultLength;
            }
            return CompareAngle(a, b);
        }

        private static int ComapreLength(Point a, Point b)
        {
            var lengthA = a.LengthSqr;
            var lenghtB = b.LengthSqr;

            return lenghtB.CompareTo(lengthA);
        }

        private int CompareAngle(Point a, Point b)
        {
            var deltaA = Helpers.DeltaAngle(_angle, a.Angle);
            var deltaB = Helpers.DeltaAngle(_angle, b.Angle);

            var result = deltaA.CompareTo(deltaB);
            return result;
        }
    }
}
