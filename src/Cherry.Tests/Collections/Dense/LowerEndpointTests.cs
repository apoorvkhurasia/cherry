using Cherry.Collections.Dense;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class LowerEndpointTests
    {
        [TestMethod]
        public void TestInfiniteExclusiveException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => LED.FiniteInclusive(double.NegativeInfinity));
            Assert.ThrowsException<ArgumentException>(
                () => LED.FiniteExclusive(double.PositiveInfinity));
        }

        [TestMethod]
        public void TestValidEndpointCreation()
        {
            LED negInf = LED.FiniteExclusive(double.NegativeInfinity);
            Assert.IsFalse(negInf.IsFinite);
            Assert.IsFalse(negInf.IsInclusive);
            Assert.AreEqual(LED.NegativeInfinity(), negInf);
            AssertOrderingEquals(LED.NegativeInfinity(), negInf);
        }

        [TestMethod]
        public void TestComparisionToLowerEndpoints()
        {
            AssertOrderingEquals(
                LED.NegativeInfinity(), LED.NegativeInfinity());

            AssertOrderingLessThan(
                LED.NegativeInfinity(), LED.FiniteExclusive(1));

            AssertOrderingLessThan(
                LED.NegativeInfinity(), LED.FiniteInclusive(1));

            AssertOrderingLessThan(
                LED.FiniteExclusive(10), LED.FiniteExclusive(11));

            AssertOrderingLessThan(
                LED.FiniteInclusive(10), LED.FiniteExclusive(10));
        }

        [TestMethod]
        public void TestComparisionToUpperEndpoints()
        {
            AssertOrderingLessThan(
                LED.NegativeInfinity(), UED.PositiveInfinity());

            AssertOrderingLessThan(
                LED.NegativeInfinity(), LED.FiniteInclusive(1));

            AssertOrderingLessThan(
                LED.FiniteExclusive(10), LED.FiniteExclusive(11));

            AssertOrderingLessThan(
                LED.FiniteInclusive(10), LED.FiniteExclusive(10));
        }

        private static void AssertOrderingEquals<T>(
            LowerEndpoint<T> one, LowerEndpoint<T> two)
            where T : IComparable<T>
        {
            Assert.IsTrue(one.Equals(two));
            Assert.IsTrue(two.Equals(one));

            Assert.IsTrue(one == two);
            Assert.IsTrue(two == one);

            Assert.IsFalse(one != two);
            Assert.IsFalse(two != one);

            Assert.IsTrue(two <= one);
            Assert.IsTrue(one <= two);

            Assert.IsTrue(two >= one);
            Assert.IsTrue(one >= two);

            Assert.IsFalse(two < one);
            Assert.IsFalse(one < two);

            Assert.IsFalse(two > one);
            Assert.IsFalse(one > two);
        }

        private static void AssertOrderingEquals<T>(
            LowerEndpoint<T> one, T two) where T : IComparable<T>
        {
            Assert.IsTrue(one == two);
            Assert.IsTrue(two == one);

            Assert.IsFalse(one != two);
            Assert.IsFalse(two != one);

            Assert.IsTrue(two <= one);
            Assert.IsTrue(one <= two);

            Assert.IsTrue(two >= one);
            Assert.IsTrue(one >= two);

            Assert.IsFalse(two < one);
            Assert.IsFalse(one < two);

            Assert.IsFalse(two > one);
            Assert.IsFalse(one > two);
        }

        private static void AssertOrderingEquals<T>(
            LowerEndpoint<T> one, UpperEndpoint<T> two) where T : IComparable<T>
        {
            Assert.IsTrue(two <= one);
            Assert.IsTrue(one <= two);

            Assert.IsTrue(two >= one);
            Assert.IsTrue(one >= two);

            Assert.IsFalse(two < one);
            Assert.IsFalse(one < two);

            Assert.IsFalse(two > one);
            Assert.IsFalse(one > two);
        }

        private static void AssertOrderingLessThan<T>(
            LowerEndpoint<T> smaller, LowerEndpoint<T> bigger)
            where T : IComparable<T>
        {
            Assert.IsTrue(bigger != smaller);
            Assert.IsTrue(smaller != bigger);

            Assert.IsFalse(bigger == smaller);
            Assert.IsFalse(smaller == bigger);

            Assert.IsTrue(smaller <= bigger);
            Assert.IsFalse(bigger <= smaller);

            Assert.IsTrue(smaller < bigger);            
            Assert.IsFalse(bigger < smaller);

            Assert.IsTrue(bigger >= smaller);
            Assert.IsFalse(smaller >= bigger);

            Assert.IsTrue(bigger > smaller);
            Assert.IsFalse(smaller > bigger);
        }

        private static void AssertOrderingLessThan<T>(
            LowerEndpoint<T> smaller, T bigger)
            where T : IComparable<T>
        {
            Assert.IsTrue(bigger != smaller);
            Assert.IsTrue(smaller != bigger);

            Assert.IsFalse(bigger == smaller);
            Assert.IsFalse(smaller == bigger);

            Assert.IsTrue(smaller <= bigger);
            Assert.IsFalse(bigger <= smaller);

            Assert.IsTrue(smaller < bigger);
            Assert.IsFalse(bigger < smaller);

            Assert.IsTrue(bigger >= smaller);
            Assert.IsFalse(smaller >= bigger);

            Assert.IsTrue(bigger > smaller);
            Assert.IsFalse(smaller > bigger);
        }

        private static void AssertOrderingLessThan<T>(
            LowerEndpoint<T> smaller, UpperEndpoint<T> bigger)
            where T : IComparable<T>
        {
            Assert.IsTrue(smaller <= bigger);
            Assert.IsFalse(bigger <= smaller);

            Assert.IsTrue(smaller < bigger);
            Assert.IsFalse(bigger < smaller);

            Assert.IsTrue(bigger >= smaller);
            Assert.IsFalse(smaller >= bigger);

            Assert.IsTrue(bigger > smaller);
            Assert.IsFalse(smaller > bigger);
        }
    }
}
