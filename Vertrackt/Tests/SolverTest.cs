using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using NUnit.Framework;
using Vertrackt.Geometry;
using Vertrackt.Solver;

namespace Vertrackt.Tests
{
    [TestFixture]
    public static class SolverTest
    {
        [TestCaseSource(nameof(TestCasesSimple))]
        public static void TestSimple(Point start, Point end, int steps)
        {
            var result = Solver.Solver.DoIt(new Description(new Car(start), end, steps));

            var shouldBeEnd = result.Solution.Aggregate(new Car(start), (car, acc) => car.Iterate(acc.Direction));

            shouldBeEnd.Position.Should().Be(end);
            shouldBeEnd.Speed.Should().Be(Point.Zero);


            Console.WriteLine("Druchläufe " + result.Loops);
        }

        private static IEnumerable<object> TestCasesSimple()
        {
            yield return new object[] { new Point(0, 0), new Point(0, 20), 4 };
            yield return new object[] { new Point(0, 0), new Point(0, 10), 4 };

            yield return new object[] { new Point(0, 10), new Point(0, 0), 4 };
            yield return new object[] { new Point(0, 20), new Point(0, 0), 4 };

            yield return new object[] { new Point(10, 10), new Point(0, 0), 4 };
            yield return new object[] { new Point(14, 16), new Point(0, 0), 7 };

            //  yield return new object[] { new Point(150, 0), new Point(0, 0), 9 };
        }


        [TestCaseSource(nameof(TestCasesObstacle))]
        public static void TestObstacle(Point start, Point end, int steps, IEnumerable<LineD> obstacles, IBoundingBox bb)
        {
            var result = Solver.Solver.DoIt(new Description(new Car(start), end, obstacles.ToList(), bb, steps));

            var shouldBeEnd = result.Solution.Aggregate(new Car(start), (car, acc) => car.Iterate(acc.Direction));

            shouldBeEnd.Position.Should().Be(end);
            shouldBeEnd.Speed.Should().Be(Point.Zero);

            Console.WriteLine("Druchläufe " + result.Loops);
        }


        private static IEnumerable<object> TestCasesObstacle()
        {
            yield return new object[]
            {
                new Point(20, 0),
                new Point(0, 0), 15,
                new[]
                {
                    new LineD(new PointD(160, -5), new PointD(-5, -5)),
                    new LineD(new PointD(160, 1), new PointD(-5, 1)),
                    new LineD(new PointD(10, 2), new PointD(10, -2)),
                },
                new BoundingBox(new Point(20, 0), new Point(0,0)).Inflate(5),
            };

            yield return new object[]
            {
                new Point(40, 0),
                new Point(0, 0), 15,
                new[]
                {
                    new LineD(new PointD(160, -5), new PointD(-5, -5)),
                    new LineD(new PointD(160, 1), new PointD(-5, 1)),
                    new LineD(new PointD(10, 2), new PointD(10, -2)),
                },
                new BoundingBox(new Point(40, 0), new Point(0,0)).Inflate(0,0, 5, 1),
            };

            yield return new object[]
         {
                new Point(60, 0),
                new Point(0, 0), 15,
                new[]
                {
                    new LineD(new PointD(160, -5), new PointD(-5, -5)),
                    new LineD(new PointD(160, 1), new PointD(-5, 1)),
                    new LineD(new PointD(10, 2), new PointD(10, -2)),
                },
                new BoundingBox(new Point(60, 0), new Point(0,0)).Inflate(0,0, 5, 1),
         };
        }
    }
}


