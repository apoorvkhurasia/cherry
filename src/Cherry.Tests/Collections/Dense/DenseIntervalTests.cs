using Microsoft.VisualStudio.TestTools.UnitTesting;
using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;
using DI = Cherry.Collections.Dense.DenseInterval<double>;
using System;
using System.Linq;
using System.Collections.Generic;
using Cherry.Collections.Dense;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class DenseIntervalTests
    {
        /// <summary>
        /// A detectable epsilion unlike <see cref="double.Epsilon"/>.
        /// </summary>
        private const double EPSILON = 1e-10;

        [TestMethod]
        public void TestUniverse()
        {
            var univ = new DI(LED.NegativeInfinity(), UED.PositiveInfinity());
            Assert.IsTrue(univ.IsUniverse);
            Assert.IsFalse(univ.IsEmpty);
            Assert.IsTrue(univ.Complement().IsEmpty);
            
            var complementOfComplement = univ.Complement()
                .Complement().AsDisjointIntervals();
            Assert.AreEqual(1, complementOfComplement.Count);
            Assert.IsTrue(complementOfComplement[0].IsUniverse);
            Assert.IsFalse(complementOfComplement[0].IsEmpty);
        }

        [TestMethod]
        public void TestContains()
        {
            var upToOne = new DI(LED.NegativeInfinity(), UED.Inclusive(1));
            Assert.IsFalse(upToOne.IsEmpty);
            RunContainsTests(upToOne, 
                new double[] { 1 - EPSILON, 1, -1 },
                new double[] { 1 + EPSILON, double.NegativeInfinity, 2 });

            var upToOneEx = new DI(LED.NegativeInfinity(), UED.Exclusive(1));
            Assert.IsFalse(upToOneEx.IsEmpty);
            RunContainsTests(upToOneEx,
                new double[] { 1 - EPSILON, -1 },
                new double[] { 1 + EPSILON, 1, double.NegativeInfinity, 2 });

            var moreThanOne = new DI(LED.Exclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOne.IsEmpty);
            RunContainsTests(moreThanOne,
                new double[] { 1 + EPSILON, 2 },
                new double[] { 1 - EPSILON, 1, double.PositiveInfinity });

            var moreThanOrEqOne = 
                new DI(LED.Inclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOrEqOne.IsEmpty);
            RunContainsTests(moreThanOrEqOne,
                new double[] { 1 + EPSILON, 1, 2 },
                new double[] { 1 - EPSILON, double.PositiveInfinity });

            var emptySet =
                new DI(LED.Exclusive(1), UED.Exclusive(1));
            Assert.IsTrue(emptySet.IsEmpty);
        }

        [TestMethod]
        public void TestMalformedIntervals()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new DI(LED.Exclusive(1), UED.Exclusive(-1)));
        }

        [TestMethod]
        public void TestUnion()
        {
            var interval1 = new DI(LED.Inclusive(1), UED.Inclusive(2));
            var interval2 = new DI(LED.Inclusive(3), UED.Inclusive(4));
            var union = interval1.Union(interval2);
            RunContainsTests(union,
                new double[] { 1, 1.5, 2 - EPSILON, 2, 3, 3.5, 4 - EPSILON, 4 },
                new double[] { 1 - EPSILON, 2 + EPSILON,
                               3 - EPSILON, 4 + EPSILON});

            //Reflexivity
            union = interval2.Union(interval1);
            RunContainsTests(union,
                new double[] { 1, 1.5, 2 - EPSILON, 2, 3, 3.5, 4 - EPSILON, 4 },
                new double[] { 1 - EPSILON, 2 + EPSILON,
                               3 - EPSILON, 4 + EPSILON});

            var interval3 = new DI(LED.Exclusive(2), UED.Exclusive(3));
            var union2 = interval1.Union(interval3);
            RunContainsTests(union2,
                new double[] { 1, 1.5, 2 - EPSILON, 2, 2.5, 3 - EPSILON},
                new double[] { -2, 1 - EPSILON, 3 + EPSILON, 4});

            var interval4 = interval1.Union(interval2).Union(interval3);
            var equivalent = new DI(LED.Inclusive(1), UED.Inclusive(4));
            Assert.IsTrue(interval4.SetEquals(equivalent));
        }

        [TestMethod]
        public void TestIntervalIntersection()
        {
            var interval1 = new DI(LED.Inclusive(1), UED.Inclusive(2));
            var interval2 = new DI(LED.Inclusive(3), UED.Inclusive(4));
            var intersection1 = interval1.Intersect(interval2);
            Assert.IsTrue(intersection1.IsEmpty);

            RunContainsTests(intersection1,
                new double[] { },
                new double[] { 1, 1.5, 2 - EPSILON, 2, 3, 3.5, 4 - EPSILON, 4,
                               1 - EPSILON, 2 + EPSILON, 
                               3 - EPSILON, 4 + EPSILON});

            //Reflexivity
            intersection1 = interval2.Intersect(interval1);
            Assert.IsTrue(intersection1.IsEmpty);

            RunContainsTests(intersection1,
                new double[] { },
                new double[] { 1, 1.5, 2 - EPSILON, 2, 3, 3.5, 4 - EPSILON, 4,
                               1 - EPSILON, 2 + EPSILON,
                               3 - EPSILON, 4 + EPSILON});

            var interval3 = interval1.Union(interval2);
            Assert.IsTrue(interval3.Intersect(interval1).SetEquals(interval1));
            Assert.IsTrue(interval3.Intersect(interval2).SetEquals(interval2));
        }

        private void RunContainsTests<T>(IDenseOrderedSet<T> interval, 
            IEnumerable<T> containedPoints, 
            IEnumerable<T> notContainedPoints) where T : IComparable<T>
        {
            foreach(var point in containedPoints)
            {
                Assert.IsTrue(interval.Contains(point),
                    $"Failed. Expected {interval} to contain {point}");
            }
            foreach (var point in notContainedPoints)
            {
                Assert.IsFalse(interval.Contains(point),
                    $"Failed. Expected {interval} to not contain {point}");
            }
        }
    }
}
