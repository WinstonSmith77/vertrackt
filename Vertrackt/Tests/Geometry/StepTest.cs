﻿using System;
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
            var delta = Helpers.DeltaAngle(a, b);
            delta.Should().BeApproximately(result, 1e-15);
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

      
    }
}
