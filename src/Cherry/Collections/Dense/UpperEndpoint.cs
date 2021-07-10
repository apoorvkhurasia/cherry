using System;
using System.Collections.Generic;
using System.Diagnostics;
using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Represents a upper endpoint of a <see cref="DenseInterval{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct UpperEndpoint<T>
        : IComparable<UpperEndpoint<T>>, 
          IComparable<LowerEndpoint<T>>, IEndpoint<T>
        where T : IComparable<T>
    {
        private UpperEndpoint(T? value, bool isInclusive, bool isInf)
        {
            Value = value;
            IsInclusive = isInclusive;
            IsInfinite = isInf;
        }

        /// <summary>
        /// Creates an inclusive and neccessarily finite
        /// <see cref="UpperEndpoint{T}"/> situated at the given value.
        /// See <seealso cref="TypeConfiguration"/> for more details on
        /// how to register instances representing positive or negative
        /// infinity for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The point at which the endpoint is to be
        /// created. Must not be null or infinite.</param>
        /// <returns>An inclusive <see cref="UpperEndpoint{T}"/> situated at
        /// the given value.</returns>
        /// <exception cref="ArgumentNullException">Given value
        /// cannot be null.</exception>
        /// <exception cref="ArgumentException">Given value cannot be
        /// infinite.</exception>
        public static UpperEndpoint<T> FiniteInclusive(T value)
        {
            if (TypeConfiguration.IsNegativeInfinity(value)
                || TypeConfiguration.IsPositiveInfinity(value))
            {
                throw new ArgumentException(SE.INF_INCLUSIVE_BOUND);
            }
            else if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return new(value, true, false);
        }

        /// <summary>
        /// Creates an exclusive <see cref="UpperEndpoint{T}"/> situated at the 
        /// given value.
        /// </summary>
        /// <param name="value">The point at which the endpoint is to be
        /// created. Must not be null or positive infinity.</param>
        /// <returns>An exclusive <see cref="UpperEndpoint{T}"/> situated at
        /// the given value.</returns>
        /// <exception cref="ArgumentNullException">Given value
        /// cannot be null.</exception>
        /// <exception cref="ArgumentException">Given value cannot be
        /// negative infinity.</exception>
        public static UpperEndpoint<T> Exclusive(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (TypeConfiguration.IsPositiveInfinity(value))
            {
                return PositiveInfinity();
            }
            else if (TypeConfiguration.IsNegativeInfinity(value))
            {
                throw new ArgumentException(SE.NEG_INF_UPPER_BOUND);
            }
            return new(value, false, false);
        }

        /// <summary>
        /// Returns an instance representing an exclusive 
        /// <see cref="UpperEndpoint{T}"/> sitauted at positive infinity.
        /// </summary>
        /// <returns>Returns the instance representing an exclusive 
        /// <see cref="UpperEndpoint{T}"/> situated at positive infinity.
        /// </returns>
        public static UpperEndpoint<T> PositiveInfinity()
        {
            if (TypeConfiguration.TryGetPositiveInfinity(out T infty))
            {
                return new(infty, false, true);
            }
            else
            {
                return new(default!, false, true);
            }
        }

        /// <summary>
        /// The value of the endpoint. Can be null for an infinite 
        /// endpoint if the type <typeparamref name="T"/> treats null
        /// as negative infinity. See <seealso cref="TypeConfiguration"/>.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Returns <see langword="true"/> if and only if this endpoint
        /// includes the value at which it is located. <see langword="false">
        /// otherwise.</see>
        /// </summary>
        public bool IsInclusive { get; }

        /// <summary>
        /// Returns <see langword="true"/> if and only if this endpoint
        /// is located at positive infinity. <see langword="false">
        /// otherwise.</see>
        /// </summary>
        public bool IsInfinite { get; }

        public int CompareTo(UpperEndpoint<T> other)
        {
            if (other.IsInfinite)
            {
                return this.IsInfinite ? 0 : -1;
            }
            else if (this.IsInfinite)
            {
                return other.IsInfinite ? 0 : 1;
            }
            Debug.Assert(Value is not null);
            Debug.Assert(other.Value is not null);
            var cmp = Value.CompareTo(other.Value);
            if (cmp == 0)
            {
                if (IsInclusive == other.IsInclusive) return 0;
                else if (IsInclusive && !other.IsInclusive) return 1;
                else return -1;
            }
            else
            {
                return cmp;
            }
        }

        public int CompareTo(LowerEndpoint<T> other) => -other.CompareTo(this);

        public int CompareTo(T? other)
        {
            if (other is null)
            {
                return 1;
            }
            if (TypeConfiguration.IsPositiveInfinity(other))
            {
                //Because even if the bound is pos. inf. it will be exclusive
                return -1;
            }
            else if (this.IsInfinite)
            {
                return 1;
            }
            Debug.Assert(Value is not null);
            var cmp = Value.CompareTo(other);
            if (cmp == 0)
            {
                if (IsInclusive) return 0;
                else return -1;
            }
            else
            {
                return cmp;
            }
        }

        public override bool Equals(object? obj) =>
            obj is UpperEndpoint<T> other &&
            IsInfinite == other.IsInfinite &&
            EqualityComparer<T?>.Default.Equals(Value, other.Value) &&
            IsInclusive == other.IsInclusive;

        public override int GetHashCode() =>
            HashCode.Combine(Value, IsInfinite, IsInclusive);

        public override string ToString() =>
            (IsInfinite ? "∞" : Value!.ToString()) +
            (IsInclusive ? "]" : ")");

        #region Comparision operators UE to UE

        public static bool operator
            ==(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => left.Equals(right);

        public static bool operator
            !=(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => !(left == right);

        public static bool operator
            <(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) < 0;

        public static bool operator
            <=(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) <= 0;

        public static bool operator
            >(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) > 0;

        public static bool operator
            >=(UpperEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) >= 0;

        #endregion

        #region Comparision operators UE to LE

        public static bool operator
            <(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) < 0;

        public static bool operator
            <=(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) <= 0;

        public static bool operator
            >(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) > 0;

        public static bool operator
            >=(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) >= 0;

        public static bool operator
            ==(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => left.CompareTo(right) == 0;

        public static bool operator
            !=(UpperEndpoint<T> left, LowerEndpoint<T> right)
            => !(left == right);

        #endregion

        #region Comparision operators UE to val

        public static bool operator <(UpperEndpoint<T> left, T right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(UpperEndpoint<T> left, T right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(UpperEndpoint<T> left, T right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(UpperEndpoint<T> left, T right)
            => left.CompareTo(right) >= 0;

        public static bool operator <(T left, UpperEndpoint<T> right)
            => !(right <= left);

        public static bool operator <=(T left, UpperEndpoint<T> right)
            => !(right < left);

        public static bool operator >(T left, UpperEndpoint<T> right)
            => !(right >= left);

        public static bool operator >=(T left, UpperEndpoint<T> right)
            => !(right > left);

        public static bool operator ==(UpperEndpoint<T> left, T right)
            => left.CompareTo(right) == 0;

        public static bool operator !=(UpperEndpoint<T> left, T right)
            => !(left == right);

        public static bool operator ==(T left, UpperEndpoint<T> right)
            => right == left;

        public static bool operator !=(T left, UpperEndpoint<T> right)
            => !(left == right);

        #endregion
    }
}
