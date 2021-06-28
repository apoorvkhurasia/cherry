using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cherry.Collections.Dense
{
    public class LowerEndpoint<T>
        : IComparable<LowerEndpoint<T>>, IComparable<T> where T : IComparable<T>
    {
        private readonly bool _infinite;

        private LowerEndpoint(T? value, bool isInclusive, bool infinite)
        {
            Value = value;
            IsInclusive = isInclusive;
            _infinite = infinite;
        }

        public static LowerEndpoint<T> FiniteInclusive(T value) =>
            new(value, true, false);

        public static LowerEndpoint<T> FiniteExclusive(T value) =>
            new(value, false, false);

        public static LowerEndpoint<T> InfiniteInstance { get; } =
            new(default!, false, true);

        public T? Value { get; }

        public bool IsInclusive { get; }

        public int CompareTo(LowerEndpoint<T>? other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (ReferenceEquals(this, other))
            {
                return 0;
            }
            else if (ReferenceEquals(this, InfiniteInstance))
            {
                return -1;
            }
            else if (ReferenceEquals(other, InfiniteInstance))
            {
                return 1;
            }
            Debug.Assert(Value is not null);
            var cmp = Value.CompareTo(other.Value);
            if (cmp == 0)
            {
                if (IsInclusive == other.IsInclusive) return 0;
                else if (IsInclusive && !other.IsInclusive) return -1;
                else return 1;
            }
            else
            {
                return cmp;
            }
        }

        public int CompareTo(T? other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (ReferenceEquals(this, InfiniteInstance))
            {
                return -1;
            }
            Debug.Assert(Value is not null);
            var cmp = Value.CompareTo(other);
            if (cmp == 0)
            {
                if (IsInclusive) return 0;
                else return 1;
            }
            else
            {
                return cmp;
            }
        }

        public bool IsValueOnTheEndPoint(T value) => CompareTo(value) == 0;

        public override bool Equals(object? obj) =>
            obj is LowerEndpoint<T> endpoint &&
            _infinite == endpoint._infinite &&
            EqualityComparer<T?>.Default.Equals(Value, endpoint.Value) &&
            IsInclusive == endpoint.IsInclusive;

        public override int GetHashCode() =>
            HashCode.Combine(_infinite, Value, IsInclusive);

        public static bool operator
            ==(LowerEndpoint<T> left, LowerEndpoint<T> right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator
            !=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => !(left == right);

        public static bool operator
            <(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left is null ? right is not null : left.CompareTo(right) < 0;

        public static bool operator
            <=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left is null || left.CompareTo(right) <= 0;

        public static bool operator
            >(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left is not null && left.CompareTo(right) > 0;

        public static bool operator
            >=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left is null ? right is null : left.CompareTo(right) >= 0;

        public static bool operator <(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) >= 0;

        public static bool operator <(T left, LowerEndpoint<T> right)
            => !(right >= left);

        public static bool operator <=(T left, LowerEndpoint<T> right)
            => !(right > left);

        public static bool operator >(T left, LowerEndpoint<T> right)
            => !(right <= left);

        public static bool operator >=(T left, LowerEndpoint<T> right)
            => !(right < left);

        public static bool operator
            ==(LowerEndpoint<T> left, T right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(LowerEndpoint<T> left, T right)
            => !(left == right);

        public static bool operator ==(T left, LowerEndpoint<T> right)
            => right == left;

        public static bool operator !=(T left, LowerEndpoint<T> right)
            => !(left == right);
    }
}
