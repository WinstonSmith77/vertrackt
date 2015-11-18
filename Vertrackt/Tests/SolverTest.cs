using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Vertrackt.Geometry;

namespace Vertrackt.Tests
{
    [TestFixture]
    public static class SolverTest
    {
        [Test]
        public static void Test()
        {
          var result =  Solver.DoIt(new Point(0, 0), new Point(100, 100));
        }

    }
}
