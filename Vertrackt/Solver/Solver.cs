using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public static class Solver
    {
        /*  public static Result DoIt(Point start, Point end, int maxSteps)
          {
              var allResults = new List<Result>();
              var tasks = new List<Task<Result>>();
              int numberOfThreads = 4;
              var boundingBox = new BoundingBox(start, end);

              for (int i = 0; i < numberOfThreads; i++)
              {
                  var i1 = i;
                  tasks.Add(Task.Run(() => DoItInner(start, end, maxSteps, numberOfThreads, i1, boundingBox)));
              }

              Task.WaitAll(tasks.ToArray());
              allResults.AddRange(tasks.Select(task => task.Result));
              var allLoops = tasks.Sum(task => task.Result.Loops);

              var bestResult = allResults.OrderBy(result => result.Solution.Count()).First();
              bestResult.Loops = allLoops;

              return bestResult;
          }*/

        public static Result DoIt(Point start, Point end, int maxSteps)
        {
            var boundingBox = new BoundingBox(start, end);
            return DoIt(start, end, maxSteps, boundingBox, new List<LineD>(), dummy => { });
        }

        public static Result DoIt(Point start, Point end, int steps, IEnumerable<LineD> obstacles, IBoundingBox boundingBox)
        {
            return DoIt(start, end, steps, boundingBox, obstacles.ToList(), dummy =>{}
        );
        }

        public static Result DoIt(Point start, Point end, int maxSteps, IBoundingBox bbox, List<LineD> obstacles, Action<Result> inmediateResult)
        {
            Result result = null;
            var loops = (long)0;
            try
            {
                var stepHelper = new Steps();

                var isFirst = true;

                var iterations = new Stack<Iteration>();
                var car = new Car(start);

                var remainingDelta = end - car.Position;

                for (;;)
                {
                    loops++;
                    Iteration iteration;

                    var needToTrackBack =
                        iterations.Count >= maxSteps ||
                        !bbox.IsInside(car.Position) ||
                        WrongCarState(iterations, car, end) ||
                        CrashWithObstacles(obstacles, iterations.PeekCheckNull()?.Line) ||
                        CheckIfTrackForCrossedOldTrack(iterations);

                    if (needToTrackBack)
                    {
                        var temp = TrackBackOneStep(iterations);

                        iteration = temp.Item1;
                        car = temp.Item2;
                    }
                    else
                    {
                        var direction = remainingDelta.Angle;
                        var stepsToUse = CalcSteps(remainingDelta, direction, stepHelper, !isFirst);
                        isFirst = false;
                        iteration = new Iteration(car, stepsToUse, 0, iterations.PeekCheckNull()?.Car.Position);
                    }

                    iterations.Push(iteration);
                    car = car.Iterate(iteration.Direction);

                    if (car.Position == end && car.Speed == Point.Zero)
                    {
                        result = ExtractResults(iterations);
                        inmediateResult(result);
                        maxSteps = Math.Max(1, iterations.Count - 1);

                        car = iterations.Pop().Car;
                    }
                }
            }
            catch (NoMoreSolutions)
            {
            }

            result.Loops = loops;
            return result;
        }



        private static bool CrashWithObstacles(IEnumerable<LineD> obstacles, LineD? currentTrack)
        {
            if (!currentTrack.HasValue)
            {
                return false;
            }

            return obstacles.Any(obstacle => obstacle.IntersectionAndOnBothLines(currentTrack.Value, false) != null);
        }

        private static bool WrongCarState(Stack<Iteration> iterations, Car car, Point end)
        {
            return iterations.Count > 0 &&
                   (car.Speed == Point.Zero || (car.Position == end && car.Speed.LengthSqr > Steps.MaxAcceleationSqr));
        }

        private static bool CheckIfTrackForCrossedOldTrack(Stack<Iteration> iterations)
        {
            if (iterations.Count <= 2)
            {
                return false;
            }

            var tracks = iterations.ToList().Select(iteration => iteration.Line).Where(line => line.HasValue).Select(line => line.Value).ToList();

            var currentTrack = tracks.First();
            tracks.Remove(currentTrack);

            return tracks.Any(line => currentTrack.IntersectionAndOnBothLines(line, true) != null);
        }


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper, bool includeInversed)
        {
            if (remainingDelta == Point.Zero)
            {
                return new List<Point> { Point.Zero };
            }
            return stepHelper.OrderByAngle(direction, includeInversed, 20 * Math.PI / 180).ToList();
        }

        private static Result ExtractResults(Stack<Iteration> iterations)
        {
            return new Result(iterations.Reverse().Select(item => item.Direction).ToList());
        }

        private static Tuple<Iteration, Car> TrackBackOneStep(Stack<Iteration> iterations)
        {
            var currentIteration = iterations.Pop().Next(iterations);
            var currentCar = currentIteration.Car;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
