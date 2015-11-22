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
            return DoIt(start, end, maxSteps, boundingBox, new List<LineD>(), dummy => { }, (dummy) => { }, true);
        }

        public static Result DoIt(Point start, Point end, int steps, IEnumerable<LineD> obstacles, IBoundingBox boundingBox)
        {
            return DoIt(start, end, steps, boundingBox, obstacles.ToList(), dummy => { }, (dummy) => { }, true);
        }

        public static Result DoIt(Point start, Point end, int maxSteps, IBoundingBox bbox, List<LineD> obstacles, Action<Result> imediateResult, Action<Result> info, bool skipAtFirstSolution)
        {
            Result result = null;
            var loops = (long)0;
            try
            {
                var stepHelper = new Steps();

                var iterations = new Stack<Iteration>();
                var car = new Car(start);

                Iteration currentIteration = null;
                double? lastDirection = null;
                for (;;)
                {
                    loops++;

                    var hasSichtLinie = HasSichtLine(obstacles, car.Position, end);

                    var needToTrackBack = NeedToTrackBack(end, maxSteps, bbox, obstacles, iterations, car, currentIteration);

                    if (needToTrackBack)
                    {
                        var temp = TrackBackOneStep(iterations);

                        currentIteration = temp.Item1;
                        car = temp.Item2;
                        lastDirection = null;
                    }
                    else
                    {
                        double direction;
                        currentIteration = NewDirection(out direction, end, car, lastDirection, hasSichtLinie, stepHelper);

                        lastDirection = direction;
                    }

                    iterations.Push(currentIteration);
                    car = car.Iterate(currentIteration.Direction);

                    if (car.Position == end && car.Speed == Point.Zero)
                    {
                        result = ExtractResults(iterations);
                        if (skipAtFirstSolution)
                        {
                            break;
                        }
                        imediateResult(result);
                        maxSteps = Math.Max(1, iterations.Count - 1);

                        car = iterations.Pop().CarBefore;
                    }

                    OutputInfos(info, loops, iterations);
                }
            }
            catch (NoMoreSolutions)
            {
            }

            result.Loops = loops;
            return result;
        }

        private static Iteration NewDirection(out double direction, Point end, Car car, double? lastDirection, bool hasSichtLinie,
            Steps stepHelper)
        {
            var accVector = end - car.Position - car.Speed;

            if (!lastDirection.HasValue || hasSichtLinie)
            {
                direction = accVector.Angle;
            }
            else
            {
                direction = lastDirection.Value;
            }
            var stepsToUse = CalcSteps(accVector, direction, stepHelper, hasSichtLinie);
            return new Iteration(car, stepsToUse, 0);
        }

        private static bool NeedToTrackBack(Point end, int maxSteps, IBoundingBox bbox, List<LineD> obstacles, Stack<Iteration> iterations,
            Car car, Iteration currentIteration)
        {
            var needToTrackBack =
                iterations.Count >= maxSteps ||
                !bbox.IsInside(car.Position) ||
                WrongCarState(iterations, car, end) ||
                CrashWithObstacles(obstacles, iterations.PeekCheckNull()?.Line) ||
                CheckIfTrackForCrossedOldTrack(iterations, currentIteration, car);
            return needToTrackBack;
        }

        private static void OutputInfos(Action<Result> info, long loops, Stack<Iteration> iterations)
        {
            if (loops%InfoAt == 0)
            {
                var tempResult = ExtractResults(iterations);
                tempResult.Loops = loops;
                tempResult.Percentage = CalcPercentage(iterations);
                info(tempResult);
            }
        }

        private static bool HasSichtLine(List<LineD> obstacles, Point position, Point end)
        {
            var line = new LineD(position, end);

            foreach (var obstacle in obstacles)
            {
                if (obstacle.IntersectionAndOnBothLines(line, false) != null)
                {
                    return false;
                }
            }
            return true;
        }

        private static double CalcPercentage(Stack<Iteration> iterations)
        {
            var data = iterations.Reverse().Select((it, index) => new { Percentage = (double)it.Index / it.Steps.Count, Total = it.Steps.Count }).ToList();

            var result = (double)0;

            for (int i = 0; i < data.Count; i++)
            {
                var partDone = data[i].Percentage;
                for (int j = i - 1; j >= 0; j--)
                {
                    partDone /= data[j].Total;
                }
                result += partDone;
            }

            return result;
        }

        public const int InfoAt = 500 * 1000;


        private static bool CrashWithObstacles(IEnumerable<LineD> obstacles, LineD? currentTrack)
        {
            if (!currentTrack.HasValue)
            {
                return false;
            }

            foreach (var obstacle in obstacles)
            {
                if (obstacle.IntersectionAndOnBothLines(currentTrack.Value, false) != null)
                {
                    return true;
                }
            }
            return false;
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
            var result = iterations
                   .Skip(1)
                   .Any(it => FilterCheckIfTrackForCrossedOldTrack(currentTrack, it.Line, currentPosition));

            return result;
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


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper, bool sichtLinie)
        {
            if (remainingDelta == Point.Zero)
            {
                return Steps.All;
            }
            return stepHelper.FilterByAngle(direction, sichtLinie).ToList();
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
