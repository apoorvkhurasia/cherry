using System;

namespace Cherry.Collections.Dense
{
    public interface IEndpoint<T> : IComparable<T> where T : IComparable<T>
    {
        bool IsInfinite { get; }

        bool IsInclusive { get; }

        T? Value { get; }
    }
}