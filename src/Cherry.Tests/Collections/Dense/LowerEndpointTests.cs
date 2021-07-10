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
                () => LED.Inclusive(double.NegativeInfinity));
            Assert.ThrowsException<ArgumentException>(
                () => LED.Exclusive(double.PositiveInfinity));
        }

        [TestMethod]
        public void TestNullValueException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => LowerEndpoint<string>.Inclusive(null!));

            Assert.ThrowsException<ArgumentNullException>(
                () => LowerEndpoint<string>.Exclusive(null!));
        }

        [TestMethod]
        public void TestValidEndpointCreation()
        {
            LED negInf = LED.Exclusive(double.NegativeInfinity);
            Assert.IsTrue(negInf.IsInfinite);
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
                LED.NegativeInfinity(), LED.Exclusive(1));

            AssertOrderingLessThan(
                LED.NegativeInfinity(), LED.Inclusive(1));

            AssertOrderingLessThan(
                LED.Exclusive(10), LED.Exclusive(11));

            AssertOrderingLessThan(
                LED.Inclusive(10), LED.Exclusive(10));

            AssertOrderingEquals(
                LED.Inclusive(10), LED.Inclusive(10));

            AssertOrderingEquals(
                LED.Exclusive(10), LED.Exclusive(10));
        }

        [TestMethod]
        public void TestComparisionToUpperEndpoints()
        {
            AssertOrderingLessThan(
                LED.NegativeInfinity(), UED.PositiveInfinity());

            AssertOrderingLessThan(
                LED.NegativeInfinity(), UED.Inclusive(1));

            AssertOrderingLessThan(
                UED.Exclusive(10), LED.Exclusive(10));

            AssertOrderingEquals(
                LED.Inclusive(10), UED.Inclusive(10));

            AssertOrderingLessThan(
                LED.Inclusive(10), UED.Inclusive(11));

            AssertOrderingLessThan(
                UED.Inclusive(11), LED.Inclusive(12));

            AssertOrderingLessThan(
                LED.Inclusive(10), LED.Exclusive(10));
        }

        [TestMethod]
        public void TestComparisionToValues()
        {
            AssertOrderingLessThan(
                LED.NegativeInfinity(), double.PositiveInfinity);

            AssertOrderingLessThan(
                double.NegativeInfinity, LED.NegativeInfinity());

            AssertOrderingLessThan(LED.NegativeInfinity(), 10);
            AssertOrderingLessThan(1, LED.Exclusive(1));
            AssertOrderingLessThan(LED.Exclusive(1), 2);
            AssertOrderingEquals(LED.Inclusive(1), 1);
        }

        private static void AssertOrderingEquals<T>(
            LowerEndpoint<T> one, LowerEndpoint<T> two)
            where T : IComparable<T>
        {
            Assert.IsTrue(one.Equals(two));
            Assert.IsTrue(two.Equals(one));
            Assert.AreEqual(two.GetHashCode(), one.GetHashCode());
            Assert.AreEqual(two.ToString(), one.ToString());

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
            T smaller, LowerEndpoint<T> bigger)
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

        private static void AssertOrderingLessThan<T>(
            UpperEndpoint<T> smaller, LowerEndpoint<T> bigger)
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

        private static void AssertOrderingEquals<T>(
            LowerEndpoint<T> one, UpperEndpoint<T> two) where T : IComparable<T>
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

    }
}
