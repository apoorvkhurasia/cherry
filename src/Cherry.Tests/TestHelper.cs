using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cherry.Tests
{
    internal static class TestHelper
    {
#pragma warning disable S1764 // Identical expressions should not be used on both sides of a binary operator

        public static void RunEqualityTests<T>(
            T obj, T[] candidatesForEquality,
            object[] candidatesNonEqual)
        {
            Assert.IsTrue(obj.Equals(obj), "Object must be equal to itself.");
            Assert.AreEqual(obj.GetHashCode(), obj.GetHashCode(),
                "Hashcode must be determinstic.");
            foreach (var eq in candidatesForEquality)
            {
                Assert.IsTrue(obj.Equals(eq), $"{eq} not found equal to {obj}");
                Assert.IsTrue(eq.Equals(obj), $"{eq} not found equal to {obj}");
                Assert.AreEqual(obj.GetHashCode(), eq.GetHashCode(),
                    $"{eq} and {obj} are equal but have different hashcodes.");
                Assert.AreEqual(obj.ToString(), eq.ToString(),
                    $"{eq} and {obj} are equal but have different string representations.");
                if (obj is IComparable<T> cmp)
                {
                    Assert.AreEqual(0, cmp.CompareTo(eq),
                        $"{eq} and {obj} are equal but not ordered equally.");
                }
            }

            foreach (var neq in candidatesNonEqual)
            {
                Assert.IsFalse(neq.Equals(obj), 
                    $"{neq} was found equal to {obj}");
                Assert.IsFalse(obj.Equals(neq),
                    $"{neq} was found equal to {obj}");
            }
        }
    }

#pragma warning restore S1764 // Identical expressions should not be used on both sides of a binary operator
}