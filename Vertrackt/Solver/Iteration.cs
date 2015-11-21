using System.Collections.Generic;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    class Iteration
    {
     

        public Iteration(Car carBefore, IReadOnlyList<Point> steps, int index, Point? lastCarPosition):
            this(carBefore, steps, index, lastCarPosition == null ? (LineD?) null : new LineD(carBefore.Position, lastCarPosition.Value))
        {
          
        }

        private Iteration(Car carBefore, IReadOnlyList<Point> steps, int index, LineD? line)
        {
            CarBefore = carBefore;
            Steps = steps;
            Line = line;
            Index = index;
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
                return new Iteration(CarBefore, Steps, Index + 1, Line);
            }

            if (stack.Count == 0)
            {
                throw new NoMoreSolutions();
            }

            return stack.Pop().Next(stack);
        }
    }
}