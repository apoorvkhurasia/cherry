using Microsoft.VisualStudio.TestTools.UnitTesting;
using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;
using DI = Cherry.Collections.Dense.DenseInterval<double>;
using System;

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
            Assert.IsFalse(upToOne.Contains(double.NegativeInfinity));
            Assert.IsFalse(upToOne.Contains(1 + EPSILON));
            Assert.IsTrue(upToOne.Contains(1 - EPSILON));
            Assert.IsTrue(upToOne.Contains(1));
            Assert.IsTrue(upToOne.Contains(-1));

            var upToOneEx = new DI(LED.NegativeInfinity(), UED.Exclusive(1));
            Assert.IsFalse(upToOneEx.IsEmpty);
            Assert.IsFalse(upToOneEx.Contains(double.NegativeInfinity));
            Assert.IsFalse(upToOneEx.Contains(1 + EPSILON));
            Assert.IsTrue(upToOneEx.Contains(1 - EPSILON));
            Assert.IsFalse(upToOneEx.Contains(1));
            Assert.IsTrue(upToOneEx.Contains(-1));

            var moreThanOne = new DI(LED.Exclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOne.IsEmpty);
            Assert.IsFalse(moreThanOne.Contains(double.PositiveInfinity));
            Assert.IsTrue(moreThanOne.Contains(1 + EPSILON));
            Assert.IsFalse(moreThanOne.Contains(1 - EPSILON));
            Assert.IsFalse(moreThanOne.Contains(1));
            Assert.IsTrue(moreThanOne.Contains(2));

            var moreThanOrEqOne = 
                new DI(LED.Inclusive(1), UED.PositiveInfinity());
            Assert.IsFalse(moreThanOrEqOne.IsEmpty);
            Assert.IsFalse(moreThanOrEqOne.Contains(double.PositiveInfinity));
            Assert.IsTrue(moreThanOrEqOne.Contains(1 + EPSILON));
            Assert.IsFalse(moreThanOrEqOne.Contains(1 - EPSILON));
            Assert.IsTrue(moreThanOrEqOne.Contains(1));
            Assert.IsTrue(moreThanOrEqOne.Contains(2));

            var emptySet =
                new DI(LED.Exclusive(1), UED.Exclusive(1));
            Assert.IsTrue(emptySet.IsEmpty);

            var anotherEmptySet =
                new DI(LED.Exclusive(1), UED.Exclusive(-1));
            Assert.IsTrue(anotherEmptySet.IsEmpty);
        }
    }
}
