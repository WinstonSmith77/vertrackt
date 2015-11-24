using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            AllWithoutEmpty = new List<Point>(all);
            all.Add(Point.Zero);
            All = all;
        }

        public List<Point> FilterByAngle(double angle)
        {
            return _sortedForAngle.GetValueOrCreateType(angle, () =>
            {
                var result = AllWithoutEmpty.Where(point => Filter(point)).ToList();

                result.Sort(new PointSorter(angle));

                /* if (hasSichtLinie)
                 {
                     var filtered = result.Where(point => FilterDirections(point, angle, 5 * Math.PI / 180));
                     var inverted = filtered.Reverse().Select(item => -item);

                     result = filtered.ToList();
                     result.AddRange(inverted);
                 }*/

               // result.Add(Point.Zero);

                return result;
            });
        }

        private bool Filter(Point point)
        {
            return (point.X % Solver.Solver.FilterBase == 0 && point.Y % Solver.Solver.FilterBase == 0);
        }


        private static bool FilterDirections(Point item, double direction, double tolerance)
        {
            return Helpers.DeltaAngle(item.Angle, direction) < tolerance;
        }

        private readonly Dictionary<double, List<Point>> _sortedForAngle = new Dictionary<double, List<Point>>(new CompareTuple());


        public static IReadOnlyList<Point> AllWithoutEmpty { get; }
        public static IReadOnlyList<Point> All { get; }

        private class CompareTuple : IEqualityComparer<double>
        {
            public bool Equals(double x, double y)
            {
                if (RoundToInt(x) == RoundToInt(y))
                {
                    return true;
                }

                return false;
            }

            public int GetHashCode(double tuple)
            {
                return RoundToInt(tuple).GetHashCode();
            }

            private static int RoundToInt(double value)
            {
                return (int)(value * 180 / Math.PI);
            }
        }
    }
}
