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
        public IterationStep(Car currentCar, List<Point> steps, int index)
        {
            CurrentCar = currentCar;
            Steps = steps;
            Index = index;
        }

        public Car CurrentCar { get; }

        public List<Point> Steps { get; }

        public int Index { get; }
    }

    public static class Solver
    {
        public static IEnumerable<Point> DoIt(Point start, Point end)
        {
            var iterations = new Stack<IterationStep>();
            var currentCar = new Car(start);

            for (;;)
            {
                var remainingDelta = end - currentCar.Position;
                var direction = remainingDelta.Angle;
                var distance = remainingDelta.Length;
                var steps = Steps.OrderByAngle(direction);
                var iteration = new IterationStep(currentCar, steps, 0);
                iterations.Push(iteration);

                currentCar = currentCar.Iterate(iteration.Steps[iteration.Index]);

            }

            var result = new List<Point>();

            return result;
        }
    }
}
