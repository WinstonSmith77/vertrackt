using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt.Geometry
{
    public class Steps
    {
        public const int MaxAcceleation = 10;

        public Steps()
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
                    all.Add(new Point(x, y));
                }
            }

            All = all;
        }

        public IEnumerable<Point> All { get; }
    }
}
