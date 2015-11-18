﻿using System;
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
        [Test]
        public static void Test()
        {
            var start = new Point(0, 0);
            var end = new Point(10, 0);
            var results = Solver.DoIt(start, end, 4).ToList();

            results.ForEach(result =>
            {
                var shouldBeEnd = result.Aggregate(new Car(start), (car, acc) => car.Iterate(acc));

                shouldBeEnd.Position.Should().Be(end);
                shouldBeEnd.Speed.Should().Be(Point.Zero);
            });
        }

    }
}
