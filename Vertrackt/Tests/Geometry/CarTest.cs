using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssert;
using NUnit.Framework;
using Vertrackt.Geometry;

namespace Vertrackt.Tests.Geometry
{
    [TestFixture]
    public class CarTest
    {
        [TestCaseSource(nameof(CarTestsEndSpeed))]
        public void MoveTest(Point start, Point end, Point endSpeed, Point[] steps)
        {
            var car = steps.Aggregate(new Car(start), (current, step) => current.Iterate(step));

            car.Position.ShouldBeEqualTo(end);
            car.Speed.ShouldBeEqualTo(endSpeed);
        }

        [TestCaseSource(nameof(CarTestsWithOutEndSpeed))]
        public void MoveTestWithoutEndSpeed(Point start, Point end, Point[] steps)
        {
            var car = steps.Aggregate(new Car(start), (current, step) => current.Iterate(step));

            car.Position.ShouldBeEqualTo(end);
        }

        private static IEnumerable<object> CarTestsEndSpeed()
        {
            yield return new object[] { new Point(0, 0), new Point(3, 3), new Point(1, 1), new[] { new Point(1, 1), new Point(0, 0), new Point(0, 0) } };
            yield return new object[] { new Point(10, 0), new Point(35, 0), new Point(10, 0), new[] { new Point(5, 0), new Point(5, 0), new Point(0, 0) } };
            yield return new object[] { new Point(0, 10), new Point(0, 35), new Point(0, 10), new[] { new Point(0, 5), new Point(0, 5), new Point(0, 0) } };
        }


        private static IEnumerable<object> CarTestsWithOutEndSpeed()
        {
            yield return new object[] { new Point(120, 180), new Point(165, 171), new[] { new Point(8, -6), new Point(10, 0), new Point(1, 9) } };
        }


    }
}
