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
        public static int FilterBase = 2;
        public static int ScaleDown = 1;
        public static int MaxSteps = 25;
        public static bool SwapStartAndEnd = false;

        public static Result DoIt(Description desc)
        {
            return DoIt(desc, dummy => { }, (dummy) => { }, true);
        }

        public static Result DoIt(Description desc, Action<Result> imediateResult, Action<Result> info, bool skipAtFirstSolution)
        {
            var maxSteps = desc.Steps;
            Result result = null;
            var loops = (long)0;
            try
            {
                var stepHelper = new Steps();

                var iterations = new Stack<Iteration>();
                var car = desc.Start;

                Iteration currentIteration = null;
                double? lastDirection = null;
                for (;;)
                {
                    loops++;

                    // var hasSichtLinie = HasSichtLine(desc.Obstacles, car.Position, desc.AuxEnd(car.Position));

                    var needToTrackBack = NeedToTrackBack(desc, iterations, car, currentIteration, maxSteps);

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
                        currentIteration = NewDirection(out direction, desc, car, lastDirection, stepHelper, iterations.Any() ? iterations.Peek().SumAcc : Point.Zero);

                        lastDirection = direction;
                    }

                    iterations.Push(currentIteration);
                    car = car.Iterate(currentIteration.Direction);

                    if (car.Position == desc.End && car.Speed == Point.Zero)
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

                    OutputInfos(info, loops, iterations, maxSteps);
                }
            }
            catch (NoMoreSolutions)
            {
            }

            result.Loops = loops;
            return result;
        }

        private static Iteration NewDirection(out double direction, Description desc, Car car, double? lastDirection,
            Steps stepHelper, Point sumAcc)
        {
            var accVector = desc.AuxEnd(car.Position) - car.Position - car.Speed;

            if (!lastDirection.HasValue)
            {
                direction = accVector.Angle;
            }
            else
            {
                direction = lastDirection.Value;
            }
            var stepsToUse = CalcSteps(accVector, direction, stepHelper);
            return new Iteration(car, stepsToUse, 0, sumAcc);
        }

        private static bool NeedToTrackBack(Description desc, Stack<Iteration> iterations,
            Car car, Iteration currentIteration, int maxSteps)
        {
            var needToTrackBack = iterations.Any() &&
                (
                iterations.Count >= maxSteps ||
                !desc.BoundingBox.IsInside(car.Position) ||
                IsAccSumInvalid(iterations.Count, currentIteration.SumAcc, maxSteps) ||
                IsWrongCarState(iterations.Count, car, desc, maxSteps) ||
                IsCrashWithObstacles(desc.Obstacles, iterations.PeekCheckNull()?.Line)
                ||
                CheckIfTrackForCrossedOldTrack(iterations, currentIteration, car)
                );
            return needToTrackBack;
        }

        private static bool IsAccSumInvalid(int count, Point sumAcc, int maxSteps)
        {
            if (Math.Abs(sumAcc.X) > (maxSteps - count) * Steps.MaxAcceleation)
            {
                return true;
            }


            if (Math.Abs(sumAcc.Y) > (maxSteps - count) * Steps.MaxAcceleation)
            {
                return true;
            }

            return false;
        }

        private static void OutputInfos(Action<Result> info, long loops, Stack<Iteration> iterations, int maxSteps)
        {
            if (loops % InfoAt == 0)
            {
                var tempResult = ExtractResults(iterations);
                tempResult.Loops = loops;
                tempResult.MaxSteps = maxSteps;
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


        public const int InfoAt = 500 * 1000;


        private static bool IsCrashWithObstacles(IEnumerable<LineD> obstacles, LineD? currentTrack)
        {
            if (!currentTrack.HasValue)
            {
                return false;
            }

            return obstacles.Any(obstacle => obstacle.IntersectionAndOnBothLines(currentTrack.Value, false) != null);
        }

        private static bool IsWrongCarState(int iterationsCount, Car car, Description desc, int maxSteps)
        {
            if (Car.MaxDistPossible(maxSteps - iterationsCount) < (car.Position - desc.End).Length)
            {
                return true;
            }

            return car.Speed == Point.Zero || (car.Position == desc.End && car.Speed.LengthSqr > Steps.MaxAcceleationSqr);
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


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper)
        {
            if (remainingDelta == Point.Zero)
            {
                return Steps.All;
            }
            return stepHelper.FilterByAngle(direction).ToList();
        }

        private static Result ExtractResults(Stack<Iteration> iterations)
        {
            return new Result(iterations);
        }

        private static Tuple<Iteration, Car> TrackBackOneStep(Stack<Iteration> iterations)
        {
            var currentIteration = iterations.Pop().Next(iterations);
            var currentCar = currentIteration.CarBefore;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
