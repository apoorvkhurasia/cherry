using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TC = Cherry.Collections.Dense.TypeConfiguration;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class TypeConfigurationTests
    {
        [TestMethod]
        public void TestDoubleDefault()
        {
            Assert.IsTrue(TC.TryGetPositiveInfinity(out double _));
            Assert.IsTrue(TC.TryGetNegativeInfinity(out double _));
            Assert.IsTrue(TC.IsPositiveInfinity(double.PositiveInfinity));
            Assert.IsTrue(TC.IsNegativeInfinity(double.NegativeInfinity));
        }

        [TestMethod]
        public void TestOverride()
        {
            TC.RegisterNegativeInfinityInstance(-1d);
            TC.RegisterPositiveInfinityInstance(1d);
            Assert.IsTrue(TC.TryGetPositiveInfinity(out double _));
            Assert.IsTrue(TC.TryGetNegativeInfinity(out double _));

            Assert.IsFalse(TC.IsPositiveInfinity(double.PositiveInfinity));
            Assert.IsFalse(TC.IsNegativeInfinity(double.NegativeInfinity));

            Assert.IsFalse(TC.IsPositiveInfinity(1));
            Assert.IsFalse(TC.IsNegativeInfinity(-1));
        }

        [TestMethod]
        public void TestUnregisteredTypes()
        {
            Assert.IsFalse(TC.TryGetNegativeInfinity<DateTime>(out var _));
            Assert.IsFalse(TC.TryGetPositiveInfinity<DateTime>(out var _));
        }
    }
}
