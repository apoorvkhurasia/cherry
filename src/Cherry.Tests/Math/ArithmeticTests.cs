using Cherry.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cherry.Tests.Math
{
    [TestClass]
    public class ArithmeticTests
    {
        [TestMethod]
        public void TestGCDPosNeg()
        {
            Assert.AreEqual(6, Arithmetic.GCD(12, 18));
            Assert.AreEqual(6, Arithmetic.GCD(-12, 18));
            Assert.AreEqual(6, Arithmetic.GCD(-12, -18));
            Assert.AreEqual(6, Arithmetic.GCD(12, -18));
        }

        [TestMethod]
        public void TestGCD()
        {
            var random = new Random();
            foreach(var prime in Primes.Sequence())
            {
                var num = prime * random.Next(25);
                Assert.AreEqual(prime, Arithmetic.GCD(prime, num), 
                    $"Test failed for {num}, {prime}");
            }
        }
    }
}
