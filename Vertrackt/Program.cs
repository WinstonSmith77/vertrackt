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

            Result result = cT(startTime);

            Console.WriteLine("Fertsch! " + (DateTime.Now - startTime).TotalSeconds + " Sekunden " + result.Loops.ToString("C0") + " Loops");
            Console.ReadKey();
        }

        private static void Test(DateTime startTime)
        {
            var start = new Point(120, 0);
            var end = new Point(0, 0);
            var steps = 35;
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

            var desc = new Description(start, end, lines.ToList(), bb, steps);

            Solver.Solver.DoIt(desc, LogResult(startTime, start), LogInfo(startTime, start),
                false);
        }


        private static Result cT(DateTime startTime)
        {
            var scale = Solver.Solver.ScaleDown;

            var start = new Point(120, 180);
            var end = new Point(320, 220);

            var a = new PointD(100, 200);
            var b = new PointD(100, 100);

            var c = new PointD(300, 200);
            var d = new PointD(300, 100);

            var sperrPunkt = new Point(200, 200);

            var lines = new[]
            {
               

                new LineD(a, sperrPunkt ),

                 new LineD(new PointD(250, 500), sperrPunkt ),
                new LineD(b, new PointD(200, 100) ),
                new LineD(a, b ),

                new LineD(c, new PointD(400, 200) ),
                new LineD(d, new PointD(400, 100) ),
                new LineD(d, c ),

                new LineD( new PointD(300, 300), d ),

                new LineD(new PointD(200, 100), new PointD(250, 200)), //extra
                new LineD(new PointD(250, 300), new PointD(300, 300) ),
                 new LineD(new PointD(200, 200), new PointD(200, 400) ),
                  new LineD(new PointD(250, 300), new PointD(250, 0) ),
               // new LineD(new Point(start.X - scale, start.Y +scale), new PointD(200,start.Y+  scale)),
              //  new LineD(new Point(200 - scale, 200), new PointD(280,400)),
              }.ToList();

           /* var lastSolutionLinePoints = new int[] { 24, 36, 34, 36, 40, 36, 46, 46, 46, 54, 52, 64, 58, 64, 64, 54, 64, 44, 64, 44 };
            var lastSolutionLines = LineD.CreateLists(lastSolutionLinePoints, scale, scale);
            var lastSolutionLines2 = LineD.CreateLists(lastSolutionLinePoints, scale, -scale);

            lines.AddRange(lastSolutionLines);
            lines.AddRange(lastSolutionLines2);*/

            var bb = new BoundingBox(new Point(start.X, start.Y), new Point(end.X, 330));


            /*  var boxesForProperEnd = new[]
              {
                  new BoundingBox(new Point(300, 200), new Point(500, 400)),
                  new BoundingBox(new Point(250, 300), new Point(500, 400)).Inflate(10)
              };


              Func<Point, Point> auxEnd = point =>
              {
                  if (boxesForProperEnd.Any(box => box.IsInside(point)))
                  {
                      return end;
                  }

                  return auxEndPoint;
              };*/

            // lines = new LineD[] {};
            var desc = new Description(start, end, lines.ToList(), bb, Solver.Solver.MaxSteps).ScaleDown(scale);

            return Solver.Solver.DoIt(desc, LogResult(startTime, start.ScaleDown(scale)), LogInfo(startTime, start.ScaleDown(scale)),
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
                Console.WriteLine(result.MaxSteps + " Max  Steps!");
                Console.WriteLine(result.Loops / (1000 * 1000) + " M Schleifen!");
                Console.WriteLine((result.Percentage * 100).ToString("F8") + " % erledigt");

                Console.WriteLine(((DateTime.Now - startTime).TotalMinutes / result.Percentage).ToString("F1") + " Minutes to go!");

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
