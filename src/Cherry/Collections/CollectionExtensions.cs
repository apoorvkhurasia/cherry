using System;
using System.Collections.Generic;
using System.Linq;
using SE = Cherry.StandardExceptions;

namespace Cherry.Collection
{
    /// <summary>
    /// This class provides extensions to the <see cref="IList{T}"/>
    /// implementations.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Swaps items at index1 and index2 with each other.
        /// </summary>
        /// <remarks>This method works fastest on list implementations
        /// that support random access. It will perform poorly on
        /// implementations such as <see cref="LinkedList{T}"/>.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list in which the items are to be
        /// swaped.</param>
        /// <param name="index1">index1</param>
        /// <param name="index2">index2</param>
        /// <exception cref="IndexOutOfRangeException">If either of
        /// the two indices refers to a location in the given list
        /// which is out of the list's range.</exception>
        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            list.RangeCheck(index1);
            list.RangeCheck(index2);
            if (index1 != index2)
            {
                var temp = list[index1];
                list[index1] = list[index2];
                list[index2] = temp;
            }
        }

        /// <summary>
        /// Throws <see cref="IndexOutOfRangeException"/> if the index is not
        /// in range of the list/array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list on which the index is to be
        /// checked.</param>
        /// <param name="index">The index to check.</param>
        public static void RangeCheck<T>(this IList<T> list, int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException(SE.IndexNonNegative);
            }
            if (index >= list.Count)
            {
                throw new IndexOutOfRangeException(SE.IndexLargerThanCount);
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> with the given element
        /// appended to the end of the given <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The original sequence.</param>
        /// <param name="item">The item to append to the original
        /// sequence.</param>
        public static IEnumerable<T> Append<T>(
            this IEnumerable<T> enumerable, T item) =>
            Enumerable.Concat(enumerable, Enumerable.Repeat(item, 1));
    }
}