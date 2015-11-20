using System.Collections.Generic;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    class Iteration
    {
        public Iteration(Car car, IReadOnlyList<Point> steps, int index)
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

        public Iteration Next(Stack<Iteration> stack)
        {
            var canNext = Index < Steps.Count - 1;
            if (canNext)
            {
                return new Iteration(Car, Steps, Index + 1);
            }

            if (stack.Count == 0)
            {
                throw new NoMoreSolutions();
            }

            return stack.Pop().Next(stack);
        }
    }
}