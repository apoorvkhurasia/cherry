using Cherry.Collections.Dense;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class UpperEndpointTests
    {
        [TestMethod]
        public void TestInfiniteExclusiveException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => UED.FiniteInclusive(double.PositiveInfinity));
            Assert.ThrowsException<ArgumentException>(
                () => UED.Exclusive(double.NegativeInfinity));
        }

        [TestMethod]
        public void TestNullValueException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => UpperEndpoint<string>.FiniteInclusive(null!));

            Assert.ThrowsException<ArgumentNullException>(
                () => UpperEndpoint<string>.Exclusive(null!));
        }

        [TestMethod]
        public void TestValidEndpointCreation()
        {
            UED posInf = UED.Exclusive(double.PositiveInfinity);
            Assert.IsTrue(posInf.IsInfinite);
            Assert.IsFalse(posInf.IsInclusive);
            Assert.AreEqual(UED.PositiveInfinity(), posInf);
            AssertOrderingEquals(UED.PositiveInfinity(), posInf);
        }

        [TestMethod]
        public void TestComparisionToUpperEndpoints()
        {
            AssertOrderingEquals(
                UED.PositiveInfinity(), UED.PositiveInfinity());

            AssertOrderingLessThan(
                UED.Exclusive(1), UED.PositiveInfinity());

            AssertOrderingLessThan(
                UED.FiniteInclusive(1), UED.PositiveInfinity());

            AssertOrderingLessThan(
                UED.Exclusive(10), UED.Exclusive(11));

            AssertOrderingLessThan(
                UED.Exclusive(10), UED.FiniteInclusive(10));

            AssertOrderingEquals(
                UED.FiniteInclusive(10), UED.FiniteInclusive(10));

            AssertOrderingEquals(
                UED.Exclusive(10), UED.Exclusive(10));
        }

        [TestMethod]
        public void TestComparisionToLowerEndpoints()
        {
            AssertOrderingLessThan(
                LED.NegativeInfinity(), UED.PositiveInfinity());

            AssertOrderingLessThan(
                LED.NegativeInfinity(), UED.FiniteInclusive(1));

            AssertOrderingLessThan(
                UED.Exclusive(10), LED.Exclusive(10));

            AssertOrderingEquals(
                LED.FiniteInclusive(10), UED.FiniteInclusive(10));

            AssertOrderingLessThan(
                LED.FiniteInclusive(10), UED.FiniteInclusive(11));

            AssertOrderingLessThan(
                UED.FiniteInclusive(11), LED.FiniteInclusive(12));

            AssertOrderingLessThan(
                UED.Exclusive(10), UED.FiniteInclusive(10));
        }

        [TestMethod]
        public void TestComparisionToValues()
        {
            AssertOrderingLessThan(
                UED.PositiveInfinity(), double.PositiveInfinity);

            AssertOrderingLessThan(10, UED.PositiveInfinity());
            AssertOrderingLessThan(10, UED.Exclusive(11));
            AssertOrderingLessThan(UED.Exclusive(1), 1);
            AssertOrderingLessThan(UED.Exclusive(1), 2);
            AssertOrderingEquals(UED.FiniteInclusive(1), 1);
        }

        private static void AssertOrderingEquals<T>(
            UpperEndpoint<T> one, UpperEndpoint<T> two)
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
            UpperEndpoint<T> smaller, UpperEndpoint<T> bigger)
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
            UpperEndpoint<T> one, T two) where T : IComparable<T>
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
            UpperEndpoint<T> smaller, T bigger)
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
            T smaller, UpperEndpoint<T> bigger)
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
