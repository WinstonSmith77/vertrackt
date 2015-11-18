using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vertrackt.Geometry;

namespace Vertrackt.Tests.Geometry
{
    [TestFixture]
    public class StepTest
    {
        [Test]
        public void BasicTest()
        {
            var allShorterOrEqualThan10 =
                Steps.AllWithoutEmpty.All(item => item.X * item.X + item.Y * item.Y <= Steps.MaxAcceleation * Steps.MaxAcceleation);

            var noEmpty =
                !Steps.AllWithoutEmpty.Any(item => item.X == 0 && item.Y == 0);

            allShorterOrEqualThan10.Should().BeTrue();
            noEmpty.Should().BeTrue();
        }

        [TestCaseSource(nameof(AngleDeltaTests))]
        public void TestAngleDelta(double a, double b, double result)
        {
            var delta = Steps.DeltaAngle(a, b);
            delta.Should().BeApproximately(result, 1e-15);
        }

        [TestCaseSource(nameof(AngleSortTests))]
        public void TestAngleSort(double angle)
        {
            var steps = Steps.OrderByAngle(angle).ToList();

            var numberOfItems = steps.Count;

            for (int i = 0; i < numberOfItems - 2; i++)
            {
                var a = steps[i];
                var b = steps[i + 1];
                var c = steps[i + 2];

                Steps.DeltaAngle(a.Angle, angle).Should().BeLessOrEqualTo(Steps.DeltaAngle(b.Angle, angle));
                var aAgreaterOrEqualToB = a.LengthSqr >= b.LengthSqr;
                var bAgreaterOrEqualToC = b.LengthSqr >= c.LengthSqr;

                (aAgreaterOrEqualToB || bAgreaterOrEqualToC).Should().BeTrue();
            }
        }


        private static IEnumerable<object> AngleDeltaTests()
        {
            yield return new object[] { 3 * Math.PI / 180, 1 * Math.PI / 180, 2 * Math.PI / 180 };
            yield return new object[] { 1 * Math.PI / 180, 3 * Math.PI / 180, 2 * Math.PI / 180 };

            yield return new object[] { 2 * Math.PI, 0, 0 };
            yield return new object[] { 0, 2 * Math.PI, 0 };

            yield return new object[] { 2 * Math.PI / 180, 358 * Math.PI / 180, 4 * Math.PI / 180 };
            yield return new object[] { 358 * Math.PI / 180, 2 * Math.PI / 180, 4 * Math.PI / 180 };

            yield return new object[] { 2 * Math.PI / 180, 2 * Math.PI / 180, 0 * Math.PI / 180 };
        }

        private static IEnumerable<object> AngleSortTests()
        {
            yield return new object[] { 45 * Math.PI / 180 };
            yield return new object[] { -45 * Math.PI / 180 };

        }
    }
}
