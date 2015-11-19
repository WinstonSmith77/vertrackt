using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Vertrackt.Geometry;

namespace Vertrackt
{
    class IterationStep
    {
        public IterationStep(Car car, IReadOnlyList<Point> steps, int index)
        {
            Car = car;
            Steps = steps;
            Index = index;
        }

        public Car Car { get; }

        public IReadOnlyList<Point> Steps { get; }

        public int Index { get; }

        public Point Direction => Steps[Index];

        public override string ToString()
        {
            return Direction.ToString();
        }

        public IterationStep Next(Stack<IterationStep> stack)
        {
            var canNext = Index < Steps.Count - 1;
            if (canNext)
            {
                return new IterationStep(Car, Steps, Index + 1);
            }

            if (stack.Count == 0)
            {
                throw new NoMoreSolutions();
            }

            return stack.Pop().Next(stack);
        }
    }

    public class Solver
    {
        public static IReadOnlyList<IEnumerable<Point>> DoIt(Point start, Point end, int maxSteps)
        {
            var allResults = new List<IEnumerable<Point>>();

            var tasks = new List<Task<IReadOnlyList<IEnumerable<Point>>>>();

            int numberOfThreads = 4;

            for (int i = 0; i < numberOfThreads; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() => DoItInner(start, end, maxSteps, numberOfThreads, i1)));
            }

            Task.WaitAll(tasks.ToArray());
            allResults.AddRange(tasks.SelectMany(task => task.Result));

            return allResults;
        }

        private static IReadOnlyList<IEnumerable<Point>> DoItInner(Point start, Point end, int maxSteps, int numberOfThreads, int indexThread)
        {
            var allResults = new List<List<Point>>();
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
                    IterationStep iteration;
                    if (NeedToTrackBack(iterations, maxSteps))
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
                            stepsToUse = stepsToUse.Split(numberOfThreads).ToList()[indexThread].ToList();
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

            return allResults;
        }


        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta, double direction, Steps stepHelper, bool includeInversed)
        {
            if (remainingDelta == Point.Zero)
            {
                return new List<Point> { Point.Zero };
            }
            return stepHelper.OrderByAngle(direction, includeInversed).ToList();
        }

        private static List<Point> ExtractResults(Stack<IterationStep> iterations)
        {
            return iterations.Reverse().Select(item => item.Direction).ToList();
        }

        private static bool NeedToTrackBack(Stack<IterationStep> iterations, int maxSteps)
        {
            return iterations.Count >= maxSteps;
        }

        private static Tuple<IterationStep, Car> TrackBackOneStep(Stack<IterationStep> iterations)
        {
            var currentIteration = iterations.Pop().Next(iterations);
            var currentCar = currentIteration.Car;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
