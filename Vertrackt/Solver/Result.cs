using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public class Result
    {
        public Result(Stack<Iteration>  solution)
        {
            Solution = solution.Reverse();
        }

        public IEnumerable<Iteration> Solution { get; }

        public long Loops { get; set; }
        public int MaxSteps { get; set; }
    }
}
