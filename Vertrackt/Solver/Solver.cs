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
        public static Result DoIt(Point start, Point end, int maxSteps)
        {
            var boundingBox = new BoundingBox(start, end);
            return DoIt(start, end, maxSteps, boundingBox, new List<LineD>(), dummy => { });
        }

        public static Result DoIt(Point start, Point end, int steps, IEnumerable<LineD> obstacles, IBoundingBox boundingBox)
        {
            return DoIt(start, end, steps, boundingBox, obstacles.ToList(), dummy => { }
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

                Iteration currentIteration = null;
                for (;;)
                {
                    loops++;

                    var needToTrackBack =
                        NeedToTrackBack(end, maxSteps, bbox, obstacles, iterations, car, currentIteration);

                    if (needToTrackBack)
                    {
                        var temp = TrackBackOneStep(iterations);

                        currentIteration = temp.Item1;
                        car = temp.Item2;
                    }
                    else
                    {
                        var direction = remainingDelta.Angle;
                        var stepsToUse = CalcSteps(remainingDelta, direction, stepHelper, !isFirst);
                        isFirst = false;
                        currentIteration = new Iteration(car, stepsToUse, 0, iterations.PeekCheckNull()?.CarBefore.Position);
                    }

                    iterations.Push(currentIteration);
                    car = car.Iterate(currentIteration.Direction);

                    if (car.Position == end && car.Speed == Point.Zero &&
                         !NeedToTrackBack(end, maxSteps, bbox, obstacles, iterations, car, currentIteration))
                    {
                        result = ExtractResults(iterations);
                        inmediateResult(result);
                        maxSteps = Math.Max(1, iterations.Count - 1);

                        car = iterations.Pop().CarBefore;
                    }
                }
            }
            catch (NoMoreSolutions)
            {
            }

            result.Loops = loops;
            return result;
        }

        private static bool NeedToTrackBack(Point end, int maxSteps, IBoundingBox bbox, List<LineD> obstacles, Stack<Iteration> iterations,
            Car car, Iteration currentIteration)
        {
            var needToTrackBack =
                iterations.Count >= maxSteps ||
                !bbox.IsInside(car.Position) ||
                WrongCarState(iterations, car, end) ||
                CrashWithObstacles(obstacles, iterations.PeekCheckNull()?.Line) ||
                CheckIfTrackForCrossedOldTrack(iterations, currentIteration);
            return needToTrackBack;
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

        private static bool CheckIfTrackForCrossedOldTrack(Stack<Iteration> iterations, Iteration currentIteration)
        {
            const int skipFirst = 2;
            if (iterations.Count <= skipFirst)
            {
                return false;
            }

            var currentTrack = currentIteration.Line.Value;
            var currentPosition = currentIteration.CarBefore.Position;
            var tracks = ExtractTracksButLast(iterations);

            foreach (var track in tracks)
            {
                if (currentTrack.IntersectionAndOnBothLines(track, true) != null)
                {
                    return true;
                }

                if (track.IsOnLine(currentPosition, true))
                {
                    return true;
                }
            }

            var positions = ExtractPositionsButLast(iterations);
            foreach (var position in positions)
            {
                if (currentTrack.IsOnLine(position, true))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<Point> ExtractPositionsButLast(Stack<Iteration> iterations)
        {
            var points =
                iterations.ToList()
                    .Select(iteration => iteration.CarBefore.Position).Skip(1).ToList();

            return points;
        }

        private static List<LineD> ExtractTracksButLast(Stack<Iteration> iterations)
        {
            var tracks =
                iterations.ToList()
                    .Select(iteration => iteration.Line)
                    .Where(line => line.HasValue)
                    .Select(line => line.Value).Skip(1).ToList();
           
            return tracks;
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
            var currentCar = currentIteration.CarBefore;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
