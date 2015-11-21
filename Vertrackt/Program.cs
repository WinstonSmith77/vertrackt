using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt
{
    static class Program
    {
        static void Main()
        {
            var startTime = DateTime.Now;

            var start = new Point(20, 0);
            var end = new Point(0, 0);
            var steps = 9;
            var lines = new LineD[]
            {
                new LineD(new PointD(160, -5), new PointD(-5, -5)),
                new LineD(new PointD(160, 1), new PointD(-5, 1)),
                new LineD(new PointD(10, 2), new PointD(10, -2))
            };

            var bb = new BoundingBox(start, end).Inflate(3);
            var result = Solver.Solver.DoIt(start, end, steps, bb, lines.ToList());

            var path = startTime.GetPathToSave();
            File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}ct.txt"), result.CtOutput());
            File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}dbg.csv"), result.OutputCSV(start));
        }


    }
}
