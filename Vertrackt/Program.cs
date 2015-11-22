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

            // Test(startTime);
            cT(startTime);
            Console.WriteLine("Fertsch!");
            Console.ReadKey();
        }

        private static void Test(DateTime startTime)
        {
            var start = new Point(120, 0);
            var end = new Point(0, 0);
            var steps = 100;
            var height = -2;

            var lines = new LineD[]
            {
                new LineD(new PointD(160, -5), new PointD(-5, -5)),
                new LineD(new PointD(160, 1), new PointD(-5, 1)),
                //new LineD(new PointD(10, 2), new PointD(10, -2)),
                //new LineD(new PointD(30, -10), new PointD(30, -1)),
                //new LineD(new PointD(60, 2), new PointD(60, -2)),
                //new LineD(new PointD(50, 3), new PointD(50, -3)),
                // new LineD(new PointD(100, 3), new PointD(100, -3)),
                // new LineD(new PointD(100, -3), new PointD(50, -3)),



                new LineD(new PointD(20, 2), new PointD(10, height)),
                new LineD(new PointD(60, 2), new PointD(60, height)),
                new LineD(new PointD(20, height), new PointD(60, height)),
                //   new LineD(new PointD(60, -height), new PointD(80, 0)),
            };

            var bb = new BoundingBox(start, end).Inflate(0, 0, Math.Abs(height) + 2, 1);

            Solver.Solver.DoIt(start, end, steps, bb, lines.ToList(), LogResult(startTime, start), LogInfo(startTime, start),
                false);
        }


        private static void cT(DateTime startTime)
        {
            var start = new Point(120, 180);
            var end = new Point(250, 180);
            var steps = 15;

            var a = new PointD(100, 200);
            var b = new PointD(100, 100);

            var c = new PointD(300, 200);
            var d = new PointD(300, 100);

            var lines = new LineD[]
            {
                new LineD(new PointD(250, 300), new PointD(250, 200) ),

                new LineD(a, new PointD(200, 200) ),
                new LineD(b, new PointD(200, 100) ),
                new LineD(a, b ),

                new LineD(c, new PointD(400, 200) ),
                new LineD(d, new PointD(400, 100) ),

                new LineD( new PointD(300, 300), d ),
            };

            var bb = new BoundingBox(new Point(0, 400), new Point(500, 0));

            Solver.Solver.DoIt(start, end, steps, bb, lines.ToList(), LogResult(startTime, start), LogInfo(startTime, start),
                false);
        }

        private static Action<Result> LogResult(DateTime startTime, Point start)
        {
            Action<Result> outPutResult = result =>
            {
                var path = startTime.GetPathToSave();
                File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}ct.txt"), result.CtOutput());
                File.WriteAllLines(Path.Combine(path, $"{result.Solution.Count()}dbg.csv"), result.OutputCSV(start, startTime));
            };
            return outPutResult;
        }

        private static Action<Result> LogInfo(DateTime startTime, Point start)
        {
            Action<Result> info = result =>
            {
                Console.WriteLine(result.Loops / (1000 * 1000) + "M Schleifen!");
                Console.WriteLine((result.Percentage * 100).ToString("F4") + "% erledigt");

                Console.WriteLine(((DateTime.Now - startTime).TotalMinutes / result.Percentage).ToString("F4") + "Minutes to go!");

                var allCarPos = Output.AllCarsInSolution(result, start);
                foreach (var carPos in allCarPos)
                {
                    Console.WriteLine('\t' + carPos.Position.ToString());
                }
            };
            return info;
        }
    }
}
