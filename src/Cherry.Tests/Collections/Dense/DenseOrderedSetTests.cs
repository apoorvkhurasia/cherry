using Microsoft.VisualStudio.TestTools.UnitTesting;
using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;
using System;
using System.Collections.Generic;
using Cherry.Collections.Dense;

using SM = Cherry.Collections.Dense.StandardMeasures;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class DenseOrderedSetTests
    {
        /// <summary>
        /// A detectable epsilion unlike <see cref="double.Epsilon"/>.
        /// </summary>
        private const double EPSILON = 1e-10;

        private static DenseOrderedSet<T> Of<T>(LowerEndpoint<T> le,
            UpperEndpoint<T> ue) where T : IComparable<T> 
            => DenseOrderedSet<T>.FromInterval(new(le, ue));

        [TestMethod]
        public void TestSingleIntervalSet()
        {
            var set = Of(LED.Inclusive(1), UED.Exclusive(5));
            Assert.AreEqual(4, set.GetLength(SM.Borel), EPSILON);
            RunContainsTests(set, new[] { 1, 2, 2.5, 3, 5 - EPSILON },
                new[] { -20, 0, 1 - EPSILON, 5, 6, 20 });

            var cmpl = set.Complement();
            var cmpIntvls = cmpl.AsDisjointIntervals();
            Assert.AreEqual(2, cmpIntvls.Count);
            Assert.AreEqual(LED.NegativeInfinity(), cmpIntvls[0].LowerEndpoint);
            Assert.AreEqual(UED.Exclusive(1), cmpIntvls[0].UpperEndpoint);
            Assert.AreEqual(LED.Inclusive(5), cmpIntvls[1].LowerEndpoint);
            Assert.AreEqual(UED.PositiveInfinity(), cmpIntvls[1].UpperEndpoint);
            Assert.IsTrue(double.IsPositiveInfinity(cmpl.GetLength(SM.Borel)));
            RunContainsTests(cmpl,
                new[] { -20e10, -20, 0, 1 - EPSILON, 5, 6, 20, 20e13},
                new[] { 1, 2, 2.5, 3, 5 - EPSILON });

            var set2 = Of(LED.NegativeInfinity(), UED.Inclusive(3));
            var cmpl2 = set2.Complement();
            var cmplIntvls2 = cmpl2.AsDisjointIntervals();
            Assert.AreEqual(1, cmplIntvls2.Count);
            Assert.AreEqual(LED.Exclusive(3), cmplIntvls2[0].LowerEndpoint);
            Assert.AreEqual(
                UED.PositiveInfinity(), cmplIntvls2[0].UpperEndpoint);
            Assert.IsTrue(double.IsPositiveInfinity(cmpl2.GetLength(SM.Borel)));

            RunContainsTests(cmpl2,
                new[] { 3 + EPSILON, 4, 4.5, 20e18 },
                new[] { double.NegativeInfinity, -1, 3 });

        }

        [TestMethod]
        public void TestUniverse()
        {
            var univ = Of(LED.NegativeInfinity(), UED.PositiveInfinity());
            Assert.IsFalse(univ.IsEmpty);
            Assert.IsTrue(double.IsPositiveInfinity(univ.GetLength(SM.Borel)));

            var complementOfComplement = univ.Complement()
                .Complement().AsDisjointIntervals();
            Assert.AreEqual(1, complementOfComplement.Count);
            Assert.IsTrue(complementOfComplement[0].IsUniverse);
            Assert.IsFalse(complementOfComplement[0].IsEmpty);

            var univ2 = DenseOrderedSet<double>.Union(new[] {
                new DenseInterval<double>(LED.Inclusive(1), UED.Exclusive(2)),
                new DenseInterval<double>(LED.Inclusive(3), UED.Exclusive(4)),
                new DenseInterval<double>(LED.NegativeInfinity(), UED.PositiveInfinity()),
            });
            Assert.IsTrue(univ2.GetLength(StandardMeasures.Borel) 
                == double.PositiveInfinity);
        }

        [TestMethod]
        public void TestContains()
        {
            var upToOne = Of(LED.NegativeInfinity(), UED.Inclusive(1));
            Assert.IsFalse(upToOne.IsEmpty);
            RunContainsTests(upToOne, 
                new double[] { 1 - EPSILON, 1, -1 },
                new double[] { 1 + EPSILON, double.NegativeInfinity, 2 });

            var upToOneEx = Of(LED.NegativeInfinity(), UED.Exclusive(1));
            Assert.IsFalse(upToOneEx.IsEmpty);
            RunContainsTests(upToOneEx,
                new double[] { 1 - EPSILON, -1 },
                new double[] { 1 + EPSILON, 1, double.NegativeInfinity, 2 });

            var moreThanOne = Of(LED.Exclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOne.IsEmpty);
            RunContainsTests(moreThanOne,
                new double[] { 1 + EPSILON, 2 },
                new double[] { 1 - EPSILON, 1, double.PositiveInfinity });

            var moreThanOrEqOne = Of(LED.Inclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOrEqOne.IsEmpty);
            RunContainsTests(moreThanOrEqOne,
                new double[] { 1 + EPSILON, 1, 2 },
                new double[] { 1 - EPSILON, double.PositiveInfinity });

            var emptySet = Of(LED.Exclusive(1), UED.Exclusive(1));
            Assert.IsTrue(emptySet.IsEmpty);
        }

        [TestMethod]
        public void TestMalformedIntervals()
        {
            Assert.ThrowsException<ArgumentException>(
                () => Of(LED.Exclusive(1), UED.Exclusive(-1)));
        }

        [TestMethod]
        public void TestIntervalIntersection()
        {
            var interval1 = Of(LED.Inclusive(1), UED.Inclusive(2));
            var interval2 = Of(LED.Inclusive(3), UED.Inclusive(4));
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

        [TestMethod]
        public void TestIntervalUnion()
        {
            var interval1 = Of(LED.Inclusive(1), UED.Inclusive(2));
            var interval2 = Of(LED.Inclusive(3), UED.Inclusive(4));
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

            var interval3 = Of(LED.Exclusive(2), UED.Exclusive(3));
            var union2 = interval1.Union(interval3);
            RunContainsTests(union2,
                new double[] { 1, 1.5, 2 - EPSILON, 2, 2.5, 3 - EPSILON },
                new double[] { -2, 1 - EPSILON, 3 + EPSILON, 4 });

            var interval4 = interval1.Union(interval2).Union(interval3);
            var equivalent = Of(LED.Inclusive(1), UED.Inclusive(4));
            Assert.IsTrue(interval4.SetEquals(equivalent));
        }

        private void RunContainsTests<T>(IDenseOrderedSet<T> set, 
            IEnumerable<T> containedPoints, 
            IEnumerable<T> notContainedPoints) where T : IComparable<T>
        {
            foreach(var point in containedPoints)
            {
                Assert.IsTrue(set.Contains(point),
                    $"Failed. Expected {set} to contain {point}");
            }
            foreach (var point in notContainedPoints)
            {
                Assert.IsFalse(set.Contains(point),
                    $"Failed. Expected {set} to not contain {point}");
            }
        }
    }
}
