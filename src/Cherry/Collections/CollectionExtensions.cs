using System;
using System.Collections.Generic;

using SE = Cherry.StandardExceptionMessages;

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
        /// Returns a read-only view of this set. Changes made to this set
        /// will be visible in the returned set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <returns>A read-only view of this set.</returns>
        public static ReadOnlySortedSet<T> AsReadOnly<T>(this SortedSet<T> set)
        {
            return new ReadOnlySortedSet<T>(set);
        }
    }
}