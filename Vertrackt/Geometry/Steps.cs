using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

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

        public static List<Point> OrderByAngle(double angle)
        {
            return _sortedForAngle.GetValueOrCreateType(angle, () =>
            {
                var result = new List<Point>(AllWithoutEmpty);

                result = result.Where(item => FilterDirections(item, angle)).ToList();
                result.Sort(new PointSorter(angle));

                var inverse = result.Select(item => item.Inverse).Reverse().ToList();
               
                result.Add(Point.Zero);
                result.AddRange(inverse);

                return result;
            });
        }


        private static bool FilterDirections(Point item, double direction)
        {
            return Helpers.DeltaAngle(item.Angle, direction) < 10 * Math.PI / 180;
        }

        private static readonly Dictionary<double, List<Point>> _sortedForAngle = new Dictionary<double, List<Point>>();


        public static IEnumerable<Point> AllWithoutEmpty { get; }
    }
}
