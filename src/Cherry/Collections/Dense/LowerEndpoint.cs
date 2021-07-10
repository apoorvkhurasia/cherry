using System;
using System.Collections.Generic;

using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Represents a lower endpoint of a <see cref="DenseInterval{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct LowerEndpoint<T>
        : IComparable<LowerEndpoint<T>>, 
          IComparable<UpperEndpoint<T>>, IEndpoint<T> 
        where T : IComparable<T>
    {

        private LowerEndpoint(T? value, bool isInclusive, bool isInf)
        {
            Value = value;
            IsInclusive = isInclusive;
            IsInfinite = isInf;
        }

        /// <summary>
        /// Creates an inclusive and neccessarily finite
        /// <see cref="LowerEndpoint{T}"/> situated at the given value.
        /// See <seealso cref="TypeConfiguration"/> for more details on
        /// how to register instances representing positive or negative
        /// infinity for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The point at which the endpoint is to be
        /// created. Must not be null or infinite.</param>
        /// <returns>An inclusive <see cref="LowerEndpoint{T}"/> situated at
        /// the given value.</returns>
        /// <exception cref="ArgumentNullException">Given value
        /// cannot be null.</exception>
        /// <exception cref="ArgumentException">Given value cannot be
        /// infinite.</exception>
        public static LowerEndpoint<T> Inclusive(T value)
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
        /// Creates an exclusive <see cref="LowerEndpoint{T}"/> situated at the 
        /// given value.
        /// </summary>
        /// <param name="value">The point at which the endpoint is to be
        /// created. Must not be null or positive infinity.</param>
        /// <returns>An exclusive <see cref="LowerEndpoint{T}"/> situated at
        /// the given value.</returns>
        /// <exception cref="ArgumentNullException">Given value
        /// cannot be null.</exception>
        /// <exception cref="ArgumentException">Given value cannot be
        /// positive infinity.</exception>
        public static LowerEndpoint<T> Exclusive(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
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

        /// <summary>
        /// Returns an instance representing an exclusive 
        /// <see cref="LowerEndpoint{T}"/> sitauted at negative infinity.
        /// </summary>
        /// <returns>Returns the instance representing an exclusive 
        /// <see cref="LowerEndpoint{T}"/> situated at negative infinity.
        /// </returns>
        public static LowerEndpoint<T> NegativeInfinity()
        {
            if (TypeConfiguration.TryGetNegativeInfinity(out T infty))
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
        /// is located at negative infinity. <see langword="false">
        /// otherwise.</see>
        /// </summary>
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
            var cmp = Value!.CompareTo(other.Value!);
            if (cmp != 0)
            {
                return cmp;
            }
            return this.IsInclusive && other.IsInclusive ? 0 : 1;
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

        #region Comparision operators LE to UE

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

        public static bool operator
            ==(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => left.CompareTo(right) == 0;

        public static bool operator
            !=(LowerEndpoint<T> left, UpperEndpoint<T> right)
            => !(left == right);

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
