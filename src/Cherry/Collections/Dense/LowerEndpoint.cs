using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cherry.Collections.Dense
{
    public struct LowerEndpoint<T>
        : IComparable<LowerEndpoint<T>>, IComparable<T>, IEndpoint<T> 
        where T : IComparable<T>
    {
        private readonly bool _isInf;

        private LowerEndpoint(T? value, bool isInclusive, bool isInf)
        {
            Value = value;
            IsInclusive = isInclusive;
            _isInf = isInf;
        }

        public static LowerEndpoint<T> FiniteInclusive(T value) =>
            new(value, true, false);

        public static LowerEndpoint<T> FiniteExclusive(T value) =>
            new(value, false, false);

        public static LowerEndpoint<T> NegativeInfinity() =>
            new(default!, false, true);

        public T? Value { get; }

        public bool IsInclusive { get; }

        public bool IsFinite => !_isInf;

        public int CompareTo(LowerEndpoint<T> other)
        {
            if(!this.IsFinite)
            {
                return other.IsFinite ? -1 : 0;
            }
            else if (!other.IsFinite)
            {
                return IsFinite ? 1 : 0;
            }
            Debug.Assert(Value is not null);
            Debug.Assert(other.Value is not null);
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

        public int CompareTo(UpperEndpoint<T> other)
        {
            if (!this.IsFinite)
            {
                return -1;
            }
            else if (!other.IsFinite)
            {
                return 1;
            }
            Debug.Assert(Value is not null);
            Debug.Assert(other.Value is not null);
            return Value.CompareTo(other.Value) > 0 ? 1 : -1;
        }

        public int CompareTo(T? other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (this._isInf)
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
            obj is LowerEndpoint<T> other &&
            _isInf == other._isInf &&
            EqualityComparer<T?>.Default.Equals(Value, other.Value) &&
            IsInclusive == other.IsInclusive;

        public override int GetHashCode() =>
            HashCode.Combine(Value, _isInf, IsInclusive);

        public static bool operator
            ==(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left.Equals(right);

        public static bool operator
            !=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => !(left == right);

        public static bool operator
            <(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) < 0;

        public static bool operator
            <=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) <= 0;

        public static bool operator
            >(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) > 0;

        public static bool operator
            >=(LowerEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) >= 0;

        public static bool operator
            <(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) < 0;

        public static bool operator
            <=(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) <= 0;

        public static bool operator
            >(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) > 0;

        public static bool operator
            >=(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) >= 0;

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

        public static bool operator ==(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) == 0;

        public static bool operator !=(LowerEndpoint<T> left, T right)
            => !(left == right);

        public static bool operator ==(T left, LowerEndpoint<T> right)
            => right == left;

        public static bool operator !=(T left, LowerEndpoint<T> right)
            => !(left == right);
    }
}
