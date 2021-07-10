using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cherry
{
    [TestClass]
    public class RandomExtensionTests
    {
        [TestMethod]
        public void TestRandomSign()
        {
            var random = new Random();
            var posCount = 0;
            var negCount = 0;
            int i = 1000000000;
            while (i-- > 0)
            {
                if (random.NextSign() > 0)
                {
                    posCount++;
                }
                else
                {
                    negCount++;
                }
            }
            Assert.AreEqual(1d, (double) posCount / negCount, 1.0e-3);
        }
    }
}
