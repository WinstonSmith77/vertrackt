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
    public class PointTest
    {
        [TestCaseSource(nameof(AddTests))]
        public void Add(Point a, Point b, Point result)
        {
            (a + b).Should().Be(result);
        }

        [TestCaseSource(nameof(AddTests))]
        public void Sub(Point a, Point b, Point result)
        {
            (result -b).Should().Be(a);
        }

        [TestCaseSource(nameof(EqualsTests))]
        public void Equals(Point a, Point b, bool result)
        {
            (a == b).Should().Be(result);
            (a.Equals(b)).Should().Be(result);
        }

        [TestCaseSource(nameof(EqualsTests))]
        public void CompareHashes(Point a, Point b, bool result)
        {
            (a.GetHashCode() == b.GetHashCode()).Should().Be(result);
        }

        private static IEnumerable<object> AddTests()
        {
            yield return new object[] { new Point(1, 2), new Point(2, 4), new Point(3, 6)};
            yield return new object[] { new Point(0, 0), new Point(0, 0), new Point(0, 0) };
            yield return new object[] { new Point(-3, -5), new Point(1, 2), new Point(-2, -3) };
        }

        private static IEnumerable<object> EqualsTests()
        {
            yield return new object[] { new Point(1, 2), new Point(2, 4), false };
            yield return new object[] { new Point(0, 0), new Point(0, 0), true };
            yield return new object[] { new Point(3, 5), new Point(3, 5), true };
            yield return new object[] { new Point(-3, -5), new Point(-3, -5), true };
            yield return new object[] { new Point(-3, -5), new Point(3, -5), false };
        }

    }
}
