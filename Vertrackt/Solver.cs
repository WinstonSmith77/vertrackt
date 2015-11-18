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
            return stack.Pop().Next(stack);
        }
    }

    public static class Solver
    {
        public static IEnumerable<Point> DoIt(Point start, Point end, int maxSteps)
        {
            var iterations = new Stack<IterationStep>();
            var currentCar = new Car(start);
            double? lastDirection = null;

            for (;;)
            {
                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;

                IterationStep iteration;
                if (HaveToTrackBack(iterations, maxSteps, lastDirection.HasValue && lastDirection != direction))
                {
                    var temp = TrackBackOneStep(iterations);

                    iteration = temp.Item1;
                    currentCar = temp.Item2;
                    direction = (end - currentCar.Position).Angle;
                }
                else
                {
                    var steps = CalcSteps(remainingDelta, direction);
                    iteration = new IterationStep(currentCar, steps, 0);
                }

                iterations.Push(iteration);
                currentCar = currentCar.Iterate(iteration.Direction);
                lastDirection = direction;

                if (currentCar.Position == end && currentCar.Speed == Point.Zero)
                {
                    return ExtractResults(iterations);
                }
            }
        }

        private static List<Point> CalcSteps(Point remainingDelta, double direction)
        {
            List<Point> steps;
            if (remainingDelta == Point.Zero)
            {
                steps = new List<Point> { Point.Zero };
            }
            else
            {
                steps = Steps.OrderByAngle(direction).ToList();
            }
            return steps;
        }

        private static IEnumerable<Point> ExtractResults(Stack<IterationStep> iterations)
        {
            return iterations.Reverse().Select(item => item.Direction);
        }

        private static bool HaveToTrackBack(Stack<IterationStep> iterations, int maxSteps, bool condition)
        {
            return iterations.Count > maxSteps || condition;
        }

        private static Tuple<IterationStep, Car> TrackBackOneStep(Stack<IterationStep> iterations)
        {
            var currentIteration = iterations.Pop().Next(iterations);
            var currentCar = currentIteration.Car;

            return Tuple.Create(currentIteration, currentCar);
        }
    }
}
