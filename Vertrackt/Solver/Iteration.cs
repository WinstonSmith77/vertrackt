using System.Collections.Generic;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    class Iteration
    {
        public Iteration(Car carBefore, IReadOnlyList<Point> steps, int index)
        {
            CarBefore = carBefore;
            Steps = steps;
            Index = index;
            Line = new LineD(carBefore.Position, carBefore.Iterate(Direction).Position);
        }

        public LineD? Line { get; set; }

        public Car CarBefore { get; }

        public IReadOnlyList<Point> Steps { get; }

        public int Index { get; }

        public Point Direction => Steps[Index];

        public override string ToString()
        {
            return Direction.ToString();
        }

        public Iteration Next(Stack<Iteration> stack)
        {
            var canNext = Index < Steps.Count - 1;
            if (canNext)
            {
                return new Iteration(CarBefore, Steps, Index + 1);
            }

            if (stack.Count == 0)
            {
                throw new NoMoreSolutions();
            }

            return stack.Pop().Next(stack);
        }
    }
}