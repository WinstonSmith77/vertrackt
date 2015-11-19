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

    public static class Solver
    {
        public static IReadOnlyList<IReadOnlyList<Point>> DoIt(Point start, Point end, int maxSteps)
        {
            var allResults = new List<List<Point>>();
            try
            {
                var iterations = new Stack<IterationStep>();
                var currentCar = new Car(start);

                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;
                var steps = CalcSteps(remainingDelta, direction);

                for (;;)
                {
                    IterationStep iteration;
                    if (NeedToTrackBack(iterations, maxSteps,
                        /* (currentCar.Position == end) ||  lastDirection.HasValue && lastDirection != direction)*/false))
                    {
                        var temp = TrackBackOneStep(iterations);

                        iteration = temp.Item1;
                        currentCar = temp.Item2;
                    }
                    else
                    {

                        iteration = new IterationStep(currentCar, steps, 0);
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

            return allResults.OrderBy(item => item.Count).ToList();
        }

        private static IReadOnlyList<Point> CalcSteps(Point remainingDelta,  double direction)
        {
            if (remainingDelta == Point.Zero)
            {
                return new List<Point> {Point.Zero};
            }
            return Steps.OrderByAngle(direction).ToList();
        }

        private static List<Point> ExtractResults(Stack<IterationStep> iterations)
        {
            return iterations.Reverse().Select(item => item.Direction).ToList();
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
