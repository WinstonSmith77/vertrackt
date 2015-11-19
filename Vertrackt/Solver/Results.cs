using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertrackt.Geometry;

namespace Vertrackt.Solver
{
    public class Results
    {
        public Results(IEnumerable<Result> solutions, long loops)
        {
            Loops = loops;
            Solutions = solutions;
        }

        public IEnumerable<Result> Solutions { get; }

        public long Loops { get; }
    }
}
