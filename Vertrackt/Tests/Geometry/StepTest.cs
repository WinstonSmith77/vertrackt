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
    public class StepTest
    {
        [Test]
        public void Test()
        {
            var allShorterOrEqualThan10 =
                Steps.AllWithoutEmpty.All(item => item.X * item.X + item.Y * item.Y <= Steps.MaxAcceleation * Steps.MaxAcceleation);

            var noEmpty =
                !Steps.AllWithoutEmpty.Any(item => item.X == 0 && item.Y == 0);

            allShorterOrEqualThan10.ShouldBeTrue();
            noEmpty.ShouldBeTrue();
        }
    }
}
