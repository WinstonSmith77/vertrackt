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
    public static class LineTest
    {
        [TestCaseSource(nameof(IntersectionTestsFailOnLine))]
        [TestCaseSource(nameof(IntersectionTests))]
        public static void Intersection(LineD a, LineD b, PointD result)
        {
            var intersection = a.Intersection(b);
            intersection.X.Should().BeApproximately(result.X, 1e-7);
            intersection.Y.Should().BeApproximately(result.Y, 1e-7);
        }

        [TestCaseSource(nameof(IntersectionTests))]
        public static void IntersectionOnBothLines(LineD a, LineD b, PointD result)
        {
            var intersection = a.IntersectionOnBothOfTheLines(b);

            intersection.HasValue.Should().BeTrue();

            intersection.Value.X.Should().BeApproximately(result.X, 1e-7);
            intersection.Value.Y.Should().BeApproximately(result.Y, 1e-7);
        }

        [TestCaseSource(nameof(IntersectionTestsFailOnLine))]
        public static void IntersectionOnBothLinesFail(LineD a, LineD b, PointD result)
        {
            var intersection = a.IntersectionOnBothOfTheLines(b);

            intersection.HasValue.Should().BeFalse();
        }


        private static IEnumerable<object> IntersectionTests()
        {
            yield return new object[]
            {
                new LineD(new PointD(-1,-1), new PointD(1,1) ),
                new LineD(new PointD(-1, 1), new PointD(1,-1) ),
                new PointD(0, 0)
            };
        }

        private static IEnumerable<object> IntersectionTestsFailOnLine()
        {
            yield return new object[]
           {
                new LineD(new PointD(-2, -2), new PointD(-1,-1) ),
                new LineD(new PointD(-2, 2), new PointD(-1,1) ),
                new PointD(0, 0)
           };

            yield return new object[]
        {
                new LineD(new PointD(-1,-1), new PointD(0,0) ),
                new LineD(new PointD(-1, 1), new PointD(0,0) ),
                new PointD(0, 0)
        };
        }
    }
}
