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
        public Result(IEnumerable<Point>  solution)
        {
            Solution = solution;
            Percentage = 1;
        }

        public IEnumerable<Point> Solution { get; }

        public long Loops { get; set; }
        public double Percentage { get; set; }
    }
}
