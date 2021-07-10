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

            var neg = Of(20, -41);
            Assert.AreEqual(-20, neg.Numerator);
            Assert.AreEqual(41, neg.Denominator);
        }

        [TestMethod]
        public void TestNegativeInfinity()
        {
            Assert.IsTrue(Of(-1, 0).IsNegativeInfinity);
            Assert.IsTrue(NegativeInfinity.IsNegativeInfinity);            
            Assert.IsTrue(NegativeInfinity == double.NegativeInfinity);
            
            RunEqualityTests(NegativeInfinity, NegativeInfinity);
            RunInequalityTests(NegativeInfinity, PositiveInfinity);
            RunInequalityTests(NegativeInfinity, -1000);
        }

        [TestMethod]
        public void TestPositiveInfinity()
        {
            Assert.IsTrue(Of(1, 0).IsPositiveInfinity);
            Assert.IsTrue(PositiveInfinity.IsPositiveInfinity);
            Assert.IsTrue(PositiveInfinity == double.PositiveInfinity);

            RunEqualityTests(PositiveInfinity, PositiveInfinity);
            RunInequalityTests(-1000, PositiveInfinity);
        }

        [TestMethod]
        public void TestInfinityMath()
        {
            Assert.AreEqual(NegativeInfinity, NegativeInfinity + 1);
            Assert.AreEqual(NegativeInfinity, NegativeInfinity - 1);
            Assert.AreEqual(NegativeInfinity,
                NegativeInfinity + NegativeInfinity);
            Assert.AreEqual(NegativeInfinity, 1 + NegativeInfinity);
            Assert.AreEqual(PositiveInfinity, 1 - NegativeInfinity);
            Assert.AreEqual(NegativeInfinity, 2 * NegativeInfinity);
            Assert.AreEqual(NegativeInfinity, NegativeInfinity / 2);
            Assert.AreEqual(PositiveInfinity, NegativeInfinity / -2);
            Assert.AreEqual(PositiveInfinity, -NegativeInfinity);
            Assert.AreEqual(PositiveInfinity,
                NegativeInfinity * NegativeInfinity);
            Assert.AreEqual(NegativeInfinity,
                NegativeInfinity * PositiveInfinity);

            Assert.AreEqual(NegativeInfinity, NegativeInfinity * 1);
            Assert.AreEqual(NegativeInfinity, 1 * NegativeInfinity);

            Assert.IsTrue((NegativeInfinity - NegativeInfinity).IsNaN);
            Assert.IsTrue((NegativeInfinity + PositiveInfinity).IsNaN);
            Assert.IsTrue((NegativeInfinity / PositiveInfinity).IsNaN);
            Assert.IsTrue((NegativeInfinity / NegativeInfinity).IsNaN);
            
            Assert.IsTrue((NegativeInfinity * Zero).IsNaN);
            Assert.IsTrue((PositiveInfinity * Zero).IsNaN);

            Assert.IsTrue((Zero * NegativeInfinity).IsNaN);
            Assert.IsTrue((Zero * PositiveInfinity).IsNaN);

            Assert.AreEqual(PositiveInfinity, PositiveInfinity + 1);
            Assert.AreEqual(PositiveInfinity, PositiveInfinity - 1);
            Assert.AreEqual(PositiveInfinity,
                PositiveInfinity + PositiveInfinity);
            Assert.AreEqual(PositiveInfinity, 1 + PositiveInfinity);
            Assert.AreEqual(NegativeInfinity, 1 - PositiveInfinity);
            Assert.AreEqual(PositiveInfinity, 2 * PositiveInfinity);
            Assert.AreEqual(PositiveInfinity, PositiveInfinity / 2);
            Assert.AreEqual(NegativeInfinity, PositiveInfinity / -2);
            Assert.AreEqual(NegativeInfinity, -PositiveInfinity);
            Assert.AreEqual(PositiveInfinity,
                PositiveInfinity * PositiveInfinity);
            Assert.AreEqual(PositiveInfinity, PositiveInfinity * 1);
            Assert.AreEqual(PositiveInfinity, 1 * PositiveInfinity);
            Assert.IsTrue((PositiveInfinity - PositiveInfinity).IsNaN);
            Assert.IsTrue((PositiveInfinity + NegativeInfinity).IsNaN);
            Assert.IsTrue((PositiveInfinity / NegativeInfinity).IsNaN);
            Assert.IsTrue((PositiveInfinity / PositiveInfinity).IsNaN);
        }

        [TestMethod]
        public void TestNaN()
        {
            Assert.IsTrue(Of(0, 0).IsNaN);
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
        }

        [TestMethod]
        public void TestSimplification()
        {
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                var num = Primes.NextRandom();
                var den = num + 1; //Coprime
                var factor = Primes.NextRandom() * random.NextSign();
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
        public void TestAddition()
        {
            Assert.AreEqual(Of(1, 1), Of(3, 4) + Of(1, 4));
            Assert.AreEqual(Of(1, 1), Of(1, 2) + Of(1, 2));
            Assert.AreEqual(Of(3, 2), Of(1, 2) + 1);
            Assert.AreEqual(Of(52283, 78192), Of(41, 181) + Of(191, 432));
            Assert.AreEqual(Of(5923, 19402), Of(-91, 178) + Of(89, 109));
        }

        [TestMethod]
        public void TestSubtraction()
        {
            Assert.AreEqual(Of(1, 2), Of(3, 4) - Of(1, 4));
            Assert.AreEqual(Zero, Of(1, 2) - Of(1, 2));
            Assert.AreEqual(Of(-1, 2), Of(1, 2) - 1);
            Assert.AreEqual(Of(-16859, 78192), Of(41, 181) - Of(191, 432));
        }

        [TestMethod]
        public void TestMultiplication()
        {
            Assert.AreEqual(Of(3, 16), Of(3, 4) * Of(1, 4));
            Assert.AreEqual(Zero, Of(1, 2) * Zero);
            Assert.AreEqual(Zero, Of(-1, 2) * 0);
            Assert.AreEqual(Of(1, 4), Of(-1, 2) * Of(-1, 2));
            Assert.AreEqual(Of(-1, 1), Of(-1, 2) * 2);
        }

        [TestMethod]
        public void TestComparisionOnFiniteValues()
        {
            RunInequalityTests(Of(45, 4611686018427387904), 
                               Of(23, 2305843009213693952));
            RunInequalityTests(Of(-45, 4611686018427387904),
                               Of(23, 2305843009213693952));
            RunEqualityTests(Of(1, 2),
                             Of(2305843009213693952, 4611686018427387904));
            RunEqualityTests(Of(1, 2), 0.5, 1.0e-10);
            RunEqualityTests(Of(1, -2), -0.5, 1.0e-10);
        }

        #region Comparision Helpers

        private void RunEqualityTests(Rational one, Rational two)
        {
            Assert.AreEqual(one.ToString(), two.ToString());
            Assert.AreEqual(one.GetHashCode(), two.GetHashCode());
            Assert.IsTrue(one.Equals(two));
            Assert.IsTrue(two.Equals(one));
            Assert.IsTrue(one == two);
            Assert.IsFalse(one != two);
            Assert.IsTrue(two == one);
            Assert.IsFalse(two != one);
            
            Assert.IsFalse(one > two);
            Assert.IsTrue(one >= two);
            Assert.IsFalse(one < two);
            Assert.IsTrue(one <= two);
            
            Assert.IsFalse(two < one);
            Assert.IsTrue(two <= one);
            Assert.IsFalse(two > one);
            Assert.IsTrue(two >= one);
        }

        private void RunInequalityTests(Rational small, Rational big)
        {
            Assert.IsFalse(small == big);
            Assert.IsTrue(small != big);
            Assert.IsFalse(big == small);
            Assert.IsTrue(big != small);

            Assert.IsFalse(small > big);
            Assert.IsFalse(small >= big);
            Assert.IsTrue(small < big);
            Assert.IsTrue(small <= big);

            Assert.IsFalse(big < small);
            Assert.IsFalse(big <= small);
            Assert.IsTrue(big > small);
            Assert.IsTrue(big >= small);
        }

        private void RunEqualityTests(Rational one, double two, double delta)
        {
            Assert.AreEqual(one, two, delta);

            Assert.IsFalse(one > two);
            Assert.IsFalse(one < two);

            Assert.IsFalse(two < one);
            Assert.IsFalse(two > one);
        }

        private void RunInequalityTests(Rational small, double big)
        {
            Assert.IsFalse(small == big);
            Assert.IsTrue(small != big);
            Assert.IsFalse(big == small);
            Assert.IsTrue(big != small);

            Assert.IsFalse(small > big);
            Assert.IsFalse(small >= big);
            Assert.IsTrue(small < big);
            Assert.IsTrue(small <= big);

            Assert.IsFalse(big < small);
            Assert.IsFalse(big <= small);
            Assert.IsTrue(big > small);
            Assert.IsTrue(big >= small);
        }

        private void RunInequalityTests(double small, Rational big)
        {
            Assert.IsFalse(small == big);
            Assert.IsTrue(small != big);
            Assert.IsFalse(big == small);
            Assert.IsTrue(big != small);

            Assert.IsFalse(small > big);
            Assert.IsFalse(small >= big);
            Assert.IsTrue(small < big);
            Assert.IsTrue(small <= big);

            Assert.IsFalse(big < small);
            Assert.IsFalse(big <= small);
            Assert.IsTrue(big > small);
            Assert.IsTrue(big >= small);
        }

        #endregion
    }
}
