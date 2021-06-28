using System;

namespace Cherry.Collections.Dense
{
    public class InfiniteEndpoint<T> : IEndpoint<T> where T : IComparable<T>
    {
        private readonly bool _isPositive;

        private InfiniteEndpoint(bool isPositive) => _isPositive = isPositive;

        public static InfiniteEndpoint<T> PositiveInfinity { get; } =
            new InfiniteEndpoint<T>(true);

        public static InfiniteEndpoint<T> NegativeInfinity { get; } =
            new InfiniteEndpoint<T>(false);

        public bool IsInclusive => false;

        public int CompareTo(T? other)
        {
            return _isPositive ? 1 : -1;
        }
    }
}
