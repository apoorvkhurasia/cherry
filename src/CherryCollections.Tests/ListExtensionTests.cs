using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CherryCollections.Tests
{
    [TestClass]
    public class ListExtensionTests
    {
        [TestMethod]
        public void TestSwap()
        {
            List<int> numbers = new() { 1, 2, 3, 4, 5, 6 };
            numbers.Swap(0, 4);

            Assert.AreEqual(5, numbers[0]);
            Assert.AreEqual(2, numbers[1]);
            Assert.AreEqual(3, numbers[2]);
            Assert.AreEqual(4, numbers[3]);
            Assert.AreEqual(1, numbers[4]);
            Assert.AreEqual(6, numbers[5]);
        }

        [TestMethod]
        public void EnsureIndexExceptionsOnSwap()
        {
            List<int> numbers = new() { 1, 2, 3, 4, 5, 6 };
            Assert.ThrowsException<IndexOutOfRangeException>(
                () => numbers.Swap(-1, 4));
            Assert.ThrowsException<IndexOutOfRangeException>(
                () => numbers.Swap(11, 4));
            Assert.ThrowsException<IndexOutOfRangeException>(
                () => numbers.Swap(0, -2));
            Assert.ThrowsException<IndexOutOfRangeException>(
                () => numbers.Swap(0, 12));
        }
    }
}
