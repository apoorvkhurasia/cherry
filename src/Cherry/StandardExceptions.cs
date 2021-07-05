using System;

namespace Cherry
{
    internal static class StandardExceptions
    {
        internal static readonly string ArrCopyInsufSpace = "Not enough space in the array after the start index to store contents of this queue.";
        internal static readonly string IndexNonNegative = "Index should be non-negative.";
        internal static readonly string NoElemsInCollection = "No elements in this collection.";
        internal static readonly string KeySmallerThanCurrKey = "New key is smaller than current key.";
        internal static readonly string IndexLargerThanCount = "Index must be at least one less than the number of items.";
        internal static readonly string ReadOnlyCollection = "This collection is a read-only collection";
        internal static readonly string POS_INF_LOWER_BOUND = "Positive infinity cannot be used as a lower bound.";
        internal static readonly string NEG_INF_LOWER_BOUND = "Negative infinity cannot be used as an upper bound.";
        internal static readonly string INF_INCLUSIVE_BOUND = "An infinite value cannot be used as an inclusive bound.";

        internal static void RequireNonNull(object arg, string argName)
        {
            if (arg is null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}