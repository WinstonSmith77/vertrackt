using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new Point(40, 0);
            var end = new Point(40, 0);
            var steps = 9;
            var lines = new[]
            {
                new LineD(new PointD(160, -5), new PointD(-5, -5)),
                new LineD(new PointD(160, 1), new PointD(-5, 1)),
                new LineD(new PointD(10, 2), new PointD(10, -2)),
            };
            var bb = new BoundingBox(new Point(40, 0), new Point(0, 0)).Inflate(20);
            var result = Solver.Solver.DoIt(start, end, steps, bb, lines.ToList());
        }
    }
}
