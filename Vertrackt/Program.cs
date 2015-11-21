using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;
using Vertrackt.Solver;

namespace Vertrackt
{
    static class Program
    {
        static void Main()
        {
            var startTime = DateTime.Now;

            var start = new Point(60, 0);
            var end = new Point(0, 0);
            var steps = 11;
            var lines = new LineD[]
            {
                new LineD(new PointD(160, -5), new PointD(-5, -5)),
                new LineD(new PointD(160, 1), new PointD(-5, 1)),
                new LineD(new PointD(10, 2), new PointD(10, -2)),
             //   new LineD(new PointD(30, 2), new PointD(30, -2))
            };

            var bb = new BoundingBox(start, end).Inflate(3);

            Action<Result> outPutResult = result =>
            {
                var path = startTime.GetPathToSave();
                File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}ct.txt"), result.CtOutput());
                File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}dbg.csv"), result.OutputCSV(start, startTime));
            };


            Action<Result> info = result =>
            {
                Console.WriteLine(result.Loops / (1000 * 1000) + "M Schleifen!");
                var allCarPos = Output.AllCarsInSolution(result, start);
                foreach (var carPos in allCarPos)
                {
                    Console.WriteLine('\t' + carPos.Position.ToString()); 
                }
            }
            ;
            Solver.Solver.DoIt(start, end, steps, bb, lines.ToList(), outPutResult, info);
        }


    }
}
