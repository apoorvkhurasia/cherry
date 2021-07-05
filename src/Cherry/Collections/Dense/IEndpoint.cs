﻿using System;

namespace Cherry.Collections.Dense
{
    public interface IEndpoint<T> where T : IComparable<T>
    {
        bool IsInfinite { get; }

        bool IsInclusive { get; }

        bool IsValueOnTheEndPoint(T value);

        T? Value { get; }
    }
}