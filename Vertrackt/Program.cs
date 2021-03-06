﻿using System;
using System.Collections;
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

            var desc = new Description(new Car(start), end, lines.ToList(), bb, steps);

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

            var lines = new[]
            {
                new LineD(new PointD(250, 300), new PointD(250, 150) ),

                new LineD(a, new PointD(200, 200) ),
                new LineD(b, new PointD(200, 100) ),
                new LineD(a, b ),

                new LineD(c, new PointD(400, 200) ),
                new LineD(d, new PointD(400, 100) ),

                new LineD( new PointD(300, 300), d ),



                new LineD(new PointD(250, 300), new PointD(300, 300) ),
                new LineD(new Point(start.X - scale, start.Y +scale), new PointD(start.X - scale, start.Y +scale)),
               
              }.ToList();

         /*   var lastSolutionLinePoints = new int[] { 24, 36, 34, 36, 40, 36, 46, 46, 46, 54, 52, 64, 58, 64, 64, 54, 64, 44, 64, 44 };
              var lastSolutionLines = LineD.CreateLists(lastSolutionLinePoints, scale, scale);
              var lastSolutionLines2 = LineD.CreateLists(lastSolutionLinePoints, scale, -scale);

              lines.AddRange(lastSolutionLines);
              lines.AddRange(lastSolutionLines2);*/

            var bb = new BoundingBox(new Point(start.X, start.Y), new Point(end.X + 2, 320));


            var boxesForProperEnd = new[]
            {
                  new BoundingBox(new Point(248, 250), new Point(500, 400)),
                  new BoundingBox(new Point(300, 200), new Point(500, 400))
              };


            var boxesForAux2 = new[]
            {
                  new BoundingBox(new Point(248, 250), new Point(275, 400)),
                 
              };

            var auxEndPoint = new Point(249, 300);
            var auxEndPoint2 = new Point(301, 300);
            var end1 = end;
            Func<Point, Point> auxEnd = point =>
            {
                point = point.ScaleUp(scale);
                if (boxesForAux2.Any(box => box.IsInside(point)))
                {
                    return auxEndPoint2;
                }

                if (boxesForProperEnd.Any(box => box.IsInside(point)))
                {
                    return end1;
                }

                return auxEndPoint;
            };

            // lines = new LineD[] {};

            if (Solver.Solver.SwapStartAndEnd)
            {
                Helpers.Swap(ref start, ref end);
            }
            var desc = new Description(new Car(start), end, lines.ToList(), bb, Solver.Solver.MaxSteps, auxEnd).ScaleDown(scale);

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

                var percentages = new List<double>();
                for (int i = 0; i < result.Solution.Count(); i++)
                {
                    percentages.Add(Output.CalcPercentage(result.Solution, i));
                }

                Console.WriteLine(((DateTime.Now - startTime).TotalMinutes / percentages.First()).ToString("F1") + " Minutes to go!");

                var allCarPos = Output.AllCarsInSolution(result, start);

                Console.WriteLine('\t' + allCarPos.First().Position.ToString());
                int index = 0;
                foreach (var carPos in allCarPos.Skip(1))
                {
                    Console.WriteLine('\t' + carPos.Position.ToString() + " " + (percentages[index++] * 100).ToString("F8") + "%");
                }
            };
            return info;
        }
    }
}
