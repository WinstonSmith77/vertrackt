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
            var steps = new Steps();
            var allShorterOrEqaulThan10 =
                steps.All.All(item => item.X * item.X + item.Y * item.Y <= Steps.MaxAcceleation * Steps.MaxAcceleation);

            allShorterOrEqaulThan10.ShouldBeTrue();
        }
    }
}
