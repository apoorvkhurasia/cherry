using Cherry.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using static Cherry.Math.Rational;

namespace Cherry.Tests.Math
{
    [TestClass]
    public class RationalTests
    {
        [TestMethod]
        public void TestCreation()
        {
            var r = Of(1, 2);
            Assert.AreEqual(1, r.Numerator);
            Assert.AreEqual(2, r.Denominator);
        }

        [TestMethod]
        public void TestNegativeInfinity()
        {
            Assert.IsTrue(Of(-1, 0).IsNegativeInfinity);
            Assert.IsTrue(NegativeInfinity.IsNegativeInfinity);
        }

        [TestMethod]
        public void TestPositiveInfinity()
        {
            Assert.IsTrue(Of(1, 0).IsPositiveInfinity);
        }

        [TestMethod]
        public void TestNaN()
        {
            Assert.IsTrue(Of(0, 0).IsNaN);
        }

        [TestMethod]
        public void TestSimplification()
        {
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                var num = Primes.NextRandom();
                var den = num + 1;
                var factor = random.Next(-1000, 1000);
                var rational = Of(num * factor, den * factor);
                Assert.AreEqual(num, rational.Numerator);
                Assert.AreEqual(den, rational.Denominator);
            }
        }

        [TestMethod]
        public void TestOverflowPrevention()
        {
            var bigFrac = Of(23, 2305843009213693952);
            var anotherFrac = Of(17, 4611686018427387904);
            var diff = anotherFrac - bigFrac;
            Assert.AreEqual(Of(-29, 4611686018427387904), diff);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            Assert.AreEqual(Of(1, 2), Of(3, 4) - Of(1, 4));
            Assert.AreEqual(Zero, Of(1, 2) - Of(1, 2));
            Assert.AreEqual(Of(-16859, 78192), Of(41, 181) - Of(191, 432));
        }

        [TestMethod]
        public void TestNaNContract()
        {
#pragma warning disable CA2242 // Test for NaN correctly
            Assert.IsFalse(NaN == NaN);
            Assert.IsTrue(NaN != NaN);
            Assert.IsFalse(NaN > NaN);
            Assert.IsFalse(NaN < NaN);
            Assert.IsFalse(NaN >= NaN);
            Assert.IsFalse(NaN <= NaN);

            //With implicit conversion
            Assert.IsFalse(NaN == double.NaN);
            Assert.IsTrue(NaN != double.NaN);
            Assert.IsFalse(NaN > double.NaN);
            Assert.IsFalse(NaN < double.NaN);
            Assert.IsFalse(NaN >= double.NaN);
            Assert.IsFalse(NaN <= double.NaN);
#pragma warning restore CA2242 // Test for NaN correctly

            //Arithmetic
            Assert.IsTrue((NaN + Of(1, 2)).IsNaN);
            Assert.IsTrue((NaN - Of(1, 2)).IsNaN);
            Assert.IsTrue((NaN * Of(1, 2)).IsNaN);
            Assert.IsTrue((NaN / Of(1, 2)).IsNaN);
            Assert.IsTrue(Of(23, 2305843009213693952) >
                Of(45, 4611686018427387904));
        }
    }
}
