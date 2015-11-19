using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public static class Solver
    {
        public static Results DoIt(Point start, Point end, int maxSteps)
        {
            var allResults = new List<Result>();
            var tasks = new List<Task<Results>>();
            int numberOfThreads = 4;
            var boundingBox = new BoundingBox(start, end);

            for (int i = 0; i < numberOfThreads; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() => DoItInner(start, end, maxSteps, numberOfThreads, i1, boundingBox)));
            }

            Task.WaitAll(tasks.ToArray());
            allResults.AddRange(tasks.SelectMany(task => task.Result.Solutions));
            var allLoops = tasks.Sum(task => task.Result.Loops);

            return new Results(allResults, allLoops);
        }

        private static Results DoItInner(Point start, Point end, int maxSteps, int numberOfThreads, int threadIndex, BoundingBox bbox)
        {
            var allResults = new List<Result>();
            var loops = (long)0;
            try
            {
                var stepHelper = new Steps();

                var isFirst = true;

                var iterations = new Stack<IterationStep>();
                var currentCar = new Car(start);

                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;

                for (;;)
                {
                    loops++;
                    IterationStep iteration;
                    if (NeedToTrackBack(iterations, maxSteps, !bbox.IsInside(currentCar.Position)))
                    {
                        var temp = TrackBackOneStep(iterations);

                        iteration = temp.Item1;
                        currentCar = temp.Item2;
                    }
                    else
                    {
                        var stepsToUse = CalcSteps(remainingDelta, direction, stepHelper, !isFirst);
                        if (isFirst)
                        {
                            stepsToUse = stepsToUse.Split(numberOfThreads).ToList()[threadIndex].ToList();
                            isFirst = false;
                        }
                        iteration = new IterationStep(currentCar, stepsToUse, 0);
                    }

                    iterations.Push(iteration);
                    currentCar = currentCar.Iterate(iteration.Direction);

                    if (currentCar.Position == end && currentCar.Speed == Point.Zero)
                    {
                        allResults.Add(ExtractResults(iterations));
                    }
                }
            }
            catch (NoMoreSolutions)
            {
            }

            return new Results(allResults, loops);
        }


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper, bool includeInversed)
        {
            if (remainingDelta == Point.Zero)
            {
                return new List<Point> { Point.Zero };
            }
            return stepHelper.OrderByAngle(direction, includeInversed).ToList();
        }

        private static Result ExtractResults(Stack<IterationStep> iterations)
        {
            return new Result(iterations.Reverse().Select(item => item.Direction).ToList());
        }

        private static bool NeedToTrackBack(Stack<IterationStep> iterations, int maxSteps, bool condition)
        {
            return iterations.Count >= maxSteps || condition;
        }

        private static Tuple<IterationStep, Car> TrackBackOneStep(Stack<IterationStep> iterations)
        {
            var currentIteration = iterations.Pop().Next(iterations);
            var currentCar = currentIteration.Car;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
