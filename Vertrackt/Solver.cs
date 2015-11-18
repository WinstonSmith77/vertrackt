using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt
{
    struct IterationStep
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

        public bool CanNext => Index < Direction.Length - 1;

        public IterationStep Next()
        {
            return new IterationStep(Car, Steps, Index + 1);
        }
    }

    public static class Solver
    {
        public static IEnumerable<Point> DoIt(Point start, Point end, int maxSteps)
        {
            var iterations = new Stack<IterationStep>();
            var currentCar = new Car(start);

            for (;;)
            {
                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;
                var steps = Steps.OrderByAngle(direction);

                IterationStep iteration;
                if (HaveToTrackBack(iterations, maxSteps))
                {
                    iteration = TrackBackOneStep(iterations, ref currentCar);
                }
                else
                {
                    iteration = new IterationStep(currentCar, steps, 0);
                }

                iterations.Push(iteration);
                currentCar = currentCar.Iterate(iteration.Direction);

                if (currentCar.Position == end && currentCar.Speed == Point.Zero)
                {
                    return iterations.Reverse().Select(item => item.Direction);
                }
            }
        }

        private static bool HaveToTrackBack(Stack<IterationStep> iterations, int maxSteps)
        {
            return iterations.Count > maxSteps;
        }

        private static IterationStep TrackBackOneStep(Stack<IterationStep> iterations, ref Car currentCar)
        {
            var lastIteration = iterations.Pop();
            currentCar = lastIteration.Car;

            if (lastIteration.CanNext)
            {
                return lastIteration.Next();
            }
            return TrackBackOneStep(iterations, ref currentCar);
        }
    }
}
