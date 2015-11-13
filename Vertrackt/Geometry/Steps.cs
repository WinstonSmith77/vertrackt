using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public static class Steps
    {
        public const int MaxAcceleation = 10;

        static Steps()
        {
            var all = new List<Point>();
            for (int x = -MaxAcceleation; x <= MaxAcceleation; x++)
            {
                for (int y = -MaxAcceleation; y <= MaxAcceleation; y++)
                {
                    var lengthSqr = x * x + y * y;
                    if (lengthSqr > MaxAcceleation * MaxAcceleation)
                    {
                        continue;
                    }

                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    all.Add(new Point(x, y));
                }
            }

            AllWithoutEmpty = all;
        }

        public static IEnumerable<Point> AllWithoutEmpty { get; }

        public static double DeltaAngle(double a, double b)
        {
            var delta = Math.Abs(a - b);

            return Math.Min(2 * Math.PI - delta, delta);
        }
    }
}
