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
        [TestCaseSource(nameof(IntersectionTests))]
        public static void Intersection(LineD a, LineD b, PointD result)
        {
            var intersection = a.Intersection(b);
            intersection.X.Should().BeApproximately(result.X, 1e-7);
            intersection.Y.Should().BeApproximately(result.Y, 1e-7);
        }


        private static IEnumerable<object> IntersectionTests()
        {
            yield return new object[]
            {
                new LineD(new PointD(-1, -1), new PointD(1,1) ),
                new LineD(new PointD(-1, 1), new PointD(1,-1) ),
                new PointD(0, 0)
            };
        }
    }
}
