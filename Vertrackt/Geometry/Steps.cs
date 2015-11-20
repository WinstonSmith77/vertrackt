using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace Vertrackt.Geometry
{
    public class Steps
    {
        public const int MaxAcceleation = 10;
        public const int MaxAcceleationSqr = MaxAcceleation * MaxAcceleation;

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

        public List<Point> OrderByAngle(double angle, bool includeInversed, double tolerance)
        {
            return _sortedForAngle.GetValueOrCreateType(Tuple.Create(angle, includeInversed, tolerance), () =>
            {
                var result = new List<Point>(AllWithoutEmpty);

                result = result.Where(item => FilterDirections(item, angle, tolerance)).ToList();
                result.Sort(new PointSorter(angle));

                var inverse = result.Select(item => -item).Reverse().ToList();
               
                result.Add(Point.Zero);
                if (includeInversed)
                {
                    result.AddRange(inverse);
                }

                return result;
            });
        }


        private static bool FilterDirections(Point item, double direction, double tolerance)
        {
            return Helpers.DeltaAngle(item.Angle, direction) < tolerance;
        }

        private  readonly Dictionary<Tuple<double, bool, double>, List<Point>> _sortedForAngle = new Dictionary<Tuple<double, bool, double>, List<Point>>();


        public static IEnumerable<Point> AllWithoutEmpty { get; }
    }
}
