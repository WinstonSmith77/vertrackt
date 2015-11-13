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
        [TestCaseSource(nameof(CarTests))]
        public void MoveTest(Point start, Point end, Point endSpeed, Point[] steps)
        {
            var car = new Car(start);

            car = steps.Aggregate(car, (current, step) => current.Iterate(step));

            car.Position.ShouldBeEqualTo(end);
            car.Speed.ShouldBeEqualTo(endSpeed);
        }

        private static IEnumerable<object> CarTests()
        {
            yield return new object[] { new Point(0, 0), new Point(2, 2), new Point(1, 1), new[] { new Point(1, 1), new Point(0, 0), new Point(0, 0) } };
            yield return new object[] { new Point(10, 0), new Point(25, 0), new Point(10, 0), new[] { new Point(5, 0), new Point(5, 0), new Point(0, 0) } };
            yield return new object[] { new Point(0, 10), new Point(0, 25), new Point(0, 10), new[] { new Point(0, 5), new Point(0, 5), new Point(0, 0) } };
        }


    }
}
