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

        public IterationStep Next()
        {
            return new IterationStep(Car, Steps, Index + 1);
        }
    }

    public static class Solver
    {
        public static IEnumerable<Point> DoIt(Point start, Point end)
        {
            var iterations = new Stack<IterationStep>();
            var currentCar = new Car(start);
            var lastDistance = double.MaxValue;

            for (;;)
            {
                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;
                var steps = Steps.OrderByAngle(direction);
                var distance = remainingDelta.Length;

                IterationStep iteration;
                if (distance >= lastDistance)
                {
                    var lastIteration = iterations.Pop();
                    currentCar = lastIteration.Car;
                    iteration = lastIteration.Next();
                }
                else
                {
                    iteration = new IterationStep(currentCar, steps, 0);
                }

              
                iterations.Push(iteration);

                currentCar = currentCar.Iterate(iteration.Direction);
                lastDistance = distance;
            }

            var result = new List<Point>();

            return result;
        }
    }
}
