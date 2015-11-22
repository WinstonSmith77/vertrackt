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
            return DoIt(start, end, maxSteps, boundingBox, new List<LineD>(), dummy => { }, (dummy) => { });
        }

        public static Result DoIt(Point start, Point end, int steps, IEnumerable<LineD> obstacles, IBoundingBox boundingBox)
        {
            return DoIt(start, end, steps, boundingBox, obstacles.ToList(), dummy => { }, (dummy) => { });
        }

        public static Result DoIt(Point start, Point end, int maxSteps, IBoundingBox bbox, List<LineD> obstacles, Action<Result> imediateResult, Action<Result> info)
        {
            Result result = null;
            var loops = (long)0;
            try
            {
                var stepHelper = new Steps();

                var iterations = new Stack<Iteration>();
                var car = new Car(start);

                Iteration currentIteration = null;
                for (;;)
                {
                    loops++;

                    var needToTrackBack =
                        iterations.Count >= maxSteps ||
                        !bbox.IsInside(car.Position) ||
                        WrongCarState(iterations, car, end) ||
                        CrashWithObstacles(obstacles, iterations.PeekCheckNull()?.Line) ||
                        CheckIfTrackForCrossedOldTrack(iterations, currentIteration, car);

                    if (needToTrackBack)
                    {
                        var temp = TrackBackOneStep(iterations);

                        currentIteration = temp.Item1;
                        car = temp.Item2;
                    }
                    else
                    {
                        var remainingDelta = end - car.Position;
                        var direction = remainingDelta.Angle;
                        var stepsToUse = CalcSteps(remainingDelta, direction, stepHelper, false);
                        currentIteration = new Iteration(car, stepsToUse, 0);
                    }

                    iterations.Push(currentIteration);
                    car = car.Iterate(currentIteration.Direction);

                    if (car.Position == end && car.Speed == Point.Zero)
                    {
                        result = ExtractResults(iterations);
                        imediateResult(result);
                        maxSteps = Math.Max(1, iterations.Count - 1);

                        car = iterations.Pop().CarBefore;
                    }


                    if (loops % InfoAt == 0)
                    {
                        var tempResult = ExtractResults(iterations);
                        tempResult.Loops = loops;
                        info(tempResult);
                    }
                }
            }
            catch (NoMoreSolutions)
            {
            }

            result.Loops = loops;
            return result;
        }

        public const int InfoAt = 5000 * 1000;


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

        private static bool CheckIfTrackForCrossedOldTrack(Stack<Iteration> iterations, Iteration currentIteration, Car currentCar)
        {
            const int skipFirst = 2;
            if (iterations.Count <= skipFirst)
            {
                return false;
            }

            var currentTrack = currentIteration.Line;
            var currentPosition = currentCar.Position;
            var tracks = ExtractTracksButLast(iterations);

            return tracks.Any(track => FilterCheckIfTrackForCrossedOldTrack(currentTrack, track, currentPosition));
        }

        private static bool FilterCheckIfTrackForCrossedOldTrack(LineD currentTrack, LineD track, Point currentPosition)
        {
            if (currentPosition == track.A)
            {
                return true;
            }

            if (currentTrack.IntersectionAndOnBothLines(track, true) != null)
            {
                return true;
            }

            if (track.IsOnLine(currentPosition, true))
            {
                return true;
            }

            if (currentTrack.IsOnLine(track.A, true))
            {
                return true;
            }

            return false;
        }


        private static List<LineD> ExtractTracksButLast(Stack<Iteration> iterations)
        {
            var tracks =
                iterations.ToList()
                    .Select(iteration => iteration.Line)
                    .Select(line => line).Skip(1).ToList();

            return tracks;
        }


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper, bool angleFirst)
        {
            if (remainingDelta == Point.Zero)
            {
                return Steps.All;
            }
            return stepHelper.OrderByAngle(direction, angleFirst).ToList();
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
