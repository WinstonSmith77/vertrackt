using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vertrackt.Geometry;

namespace Vertrackt.Tests
{
    [TestFixture]
    public static class SolverTest
    {
        [TestCaseSource(nameof(TestCases))]
        public static void Test(Point start, Point end, int steps)
        {
            var results = Solver.Solver.DoIt(start, end, steps).ToList();
            results.Count.Should().BeGreaterThan(0);

            results.ForEach(result =>
            {
                var shouldBeEnd = result.Solution.Aggregate(new Car(start), (car, acc) => car.Iterate(acc));

                shouldBeEnd.Position.Should().Be(end);
                shouldBeEnd.Speed.Should().Be(Point.Zero);
            });

            Console.WriteLine("Druchläufe " + results.GroupBy(item => item.ThreadIndex).Select(item => item.Max(item2 => item2.Loops)).Sum(item3 => item3));
           
        }

        private static IEnumerable<object> TestCases()
        {
            yield return new object[] {new Point(0, 0), new Point(0, 20), 4};
            yield return new object[] {new Point(0, 0), new Point(0, 10), 4};

            yield return new object[] {new Point(0, 10), new Point(0, 0), 4};
            yield return new object[] {new Point(0, 20), new Point(0, 0), 4};

            yield return new object[] {new Point(10, 10), new Point(0, 0), 4};
            yield return new object[] { new Point(15, 17), new Point(0, 0), 4 };

            yield return new object[] { new Point(-15, 17), new Point(9, 3), 4 };
        }
    }
}


