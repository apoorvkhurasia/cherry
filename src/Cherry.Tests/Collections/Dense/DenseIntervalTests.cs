using Microsoft.VisualStudio.TestTools.UnitTesting;
using LED = Cherry.Collections.Dense.LowerEndpoint<double>;
using UED = Cherry.Collections.Dense.UpperEndpoint<double>;
using DI = Cherry.Collections.Dense.DenseInterval<double>;

namespace Cherry.Tests.Collections.Dense
{
    [TestClass]
    public class DenseIntervalTests
    {
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
    }
}
