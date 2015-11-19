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
        private readonly int _threadIndex;

        public Result(IEnumerable<Point>  solution, long loops,int threadIndex)
        {
            _threadIndex = threadIndex;
            Loops = loops;
            Solution = solution;
        }

        public IEnumerable<Point> Solution { get; }

        public long Loops { get; }

        public int ThreadIndex
        {
            get { return _threadIndex; }
        }
    }
}
