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

        public List<Point> OrderByAngle(double angle, bool angleFirst)
        {
            return _sortedForAngle.GetValueOrCreateType(Tuple.Create(angle, angleFirst), () =>
            {
                var result = AllWithoutEmpty.Where(point => Filter(point)).ToList();
             
                result.Sort(new PointSorter(angle, angleFirst));
               
                result.Add(Point.Zero);
                
                return result;
            });
        }

        private bool Filter(Point point)
        {
           return (point.X%2 == 0 && point.Y%2 == 0);
           // return true;
        }


        /*private static bool FilterDirections(Point item, double direction, double tolerance)
        {
            return Helpers.DeltaAngle(item.Angle, direction) < tolerance;
        }*/

        private  readonly Dictionary<Tuple<double, bool>, List<Point>> _sortedForAngle = new Dictionary<Tuple<double, bool>, List<Point>>();


        public static IReadOnlyList<Point> AllWithoutEmpty { get; }
        public static IReadOnlyList<Point> All { get; }
    }
}
