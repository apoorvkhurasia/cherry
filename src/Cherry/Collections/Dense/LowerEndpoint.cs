using System;
using System.Collections.Generic;
using System.Diagnostics;

using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Represents a lower endpoint of a <see cref="DenseInterval{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct LowerEndpoint<T>
        : IComparable<LowerEndpoint<T>>, IComparable<T>, IEndpoint<T> 
        where T : IComparable<T>
    {
        private LowerEndpoint(T? value, bool isInclusive, bool isInf)
        {
            Value = value;
            IsInclusive = isInclusive;
            IsInfinite = isInf;
        }

        internal static LowerEndpoint<T> FiniteInclusive(T value)
        {
            if (TypeConfiguration.IsNegativeInfinity(value)
                || TypeConfiguration.IsPositiveInfinity(value))
            {
                throw new ArgumentException(SE.INF_INCLUSIVE_BOUND);
            }
            return new(value, true, false);
        }

        internal static LowerEndpoint<T> FiniteExclusive(T value)
        {
            if (TypeConfiguration.IsNegativeInfinity(value))
            {
                return NegativeInfinity();
            }
            else if (TypeConfiguration.IsPositiveInfinity(value))
            {
                throw new ArgumentException(SE.POS_INF_LOWER_BOUND);
            }
            return new(value, false, false);
        }

        public static LowerEndpoint<T> NegativeInfinity() =>
            new(default!, false, true);

        public T? Value { get; }

        public bool IsInclusive { get; }

        public bool IsInfinite { get; }

        public int CompareTo(LowerEndpoint<T> other)
        {
            if(this.IsInfinite)
            {
                return other.IsInfinite ? 0 : -1;
            }
            else if (other.IsInfinite)
            {
                return 1;
            }
            
            var cmp = Value!.CompareTo(other.Value!);
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
            if (this.IsInfinite || other.IsInfinite)
            {
                return -1;
            }
            return Value!.CompareTo(other.Value!) > 0 ? 1 : -1;
        }

        public int CompareTo(T? other)
        {
            if (other is null)
            {
                return 1;
            }
            
            if (TypeConfiguration.IsPositiveInfinity(other))
            {
                return -1;
            }
            else if (TypeConfiguration.IsNegativeInfinity(other))
            {
                return 1;
            }
            else if (this.IsInfinite)
            {
                return -1;
            }

            var cmp = Value!.CompareTo(other);
            if (cmp == 0)
            {
                return IsInclusive ? 0 : 1;
            }
            else
            {
                return cmp;
            }
        }

        public override bool Equals(object? obj) =>
            obj is LowerEndpoint<T> other &&
            IsInfinite == other.IsInfinite &&
            EqualityComparer<T?>.Default.Equals(Value, other.Value) &&
            IsInclusive == other.IsInclusive;

        public override int GetHashCode() =>
            HashCode.Combine(Value, IsInfinite, IsInclusive);

        public override string ToString() =>
            (IsInclusive ? "[" : "(") +
            (IsInfinite ? "-∞" : Value!.ToString());

        #region Comparision operators LE to LE

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

        #endregion

        #region Comparision operators LE and UE

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

        #endregion

        #region Comparision operators LE and Val

        public static bool operator <(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) >= 0;

        public static bool operator <(T left, LowerEndpoint<T> right)
            => !(right <= left);

        public static bool operator <=(T left, LowerEndpoint<T> right)
            => !(right < left);

        public static bool operator >(T left, LowerEndpoint<T> right)
            => !(right >= left);

        public static bool operator >=(T left, LowerEndpoint<T> right)
            => !(right > left);

        public static bool operator ==(LowerEndpoint<T> left, T right)
            => left.CompareTo(right) == 0;

        public static bool operator !=(LowerEndpoint<T> left, T right)
            => !(left == right);

        public static bool operator ==(T left, LowerEndpoint<T> right)
            => right == left;

        public static bool operator !=(T left, LowerEndpoint<T> right)
            => !(left == right);

        #endregion
    }
}
