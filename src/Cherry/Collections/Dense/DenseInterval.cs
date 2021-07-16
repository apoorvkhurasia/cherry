using System;
using System.Collections.Generic;
using System.Diagnostics;

using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// An interval is a set of dense elements (see
    /// <see cref="IDenseOrderedSet{T}"/> for details on what dense means)
    /// between two elements which define the endpoint of the interval. All
    /// elements within an interval are greater than its
    /// <see cref="LowerEndpoint"/> (or greater than or equal to when that
    /// endpoint is inclusive) and less than its <see cref="UpperEndpoint"/> (
    /// or less than or equal to when that endpoint is inclusive).
    /// </summary>
    /// <seealso cref="LowerEndpoint{T}" />
    /// <seealso cref="UpperEndpoint{T}" />
    /// <typeparam name="T"></typeparam>
    public sealed class DenseInterval<T> where T : IComparable<T>
    {
        /// <summary>
        /// Constructs an instance of this class from the given endpoints.
        /// </summary>
        /// <param name="le">The lower endpoint.</param>
        /// <param name="ue">The upper endpoint.</param>
        public DenseInterval(LowerEndpoint<T> le, UpperEndpoint<T> ue)
        {
            if (!le.IsInfinite && !ue.IsInfinite &&
                le.Value!.CompareTo(ue.Value) > 0)
            {
                throw new ArgumentException(SE.WRONG_BOUND_ORDER);
            }
            LowerEndpoint = le;
            UpperEndpoint = ue;
        }

        /// <summary>
        /// The universe set is a special instance of this class. This set
        /// contains every element of type <typeparamref name="T"/>.
        /// </summary>
        public static DenseInterval<T> Universe { get; } =
            new(LowerEndpoint<T>.NegativeInfinity(),
                UpperEndpoint<T>.PositiveInfinity());

        /// <summary>
        /// The lower endpoint of this interval.
        /// </summary>
        public LowerEndpoint<T> LowerEndpoint { get; }

        /// <summary>
        /// The upper endpoint of this interval.
        /// </summary>
        public UpperEndpoint<T> UpperEndpoint { get; }

        /// <summary>
        /// Gets the position of the given item with respect to the
        /// endpoints of this interval.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>The position of the given item with respect to the
        /// endpoints of this interval.</returns>
        public PointPosition GetPosition(T item)
        {
            if (item < LowerEndpoint)
            {
                return PointPosition.LOWER;
            }
            else if (item == LowerEndpoint)
            {
                return PointPosition.AT_LOWER_ENDPOINT;
            }
            else if (item < UpperEndpoint)
            {
                return PointPosition.WITHIN;
            }
            else if (item == UpperEndpoint)
            {
                return PointPosition.AT_UPPER_ENDPOINT;
            }
            else
            {
                return PointPosition.UPPER;
            }
        }

        /// <summary>
        /// <see langword="true"></see> if and only if this set contains no
        /// elements. <see langword="false"/> otherwise.
        /// </summary>
        public bool IsEmpty => !LowerEndpoint.IsInfinite
            && !UpperEndpoint.IsInfinite
            && Comparer<T>.Default.Compare(
                LowerEndpoint.Value, UpperEndpoint.Value) >= 0;

        /// <summary>
        /// <see langword="true"></see> if and only if this set contains every
        /// element of type <typeparamref name="T"/>. 
        /// <see langword="false"/> otherwise.
        /// </summary>
        public bool IsUniverse =>
            LowerEndpoint.IsInfinite && UpperEndpoint.IsInfinite;

        /// <summary>
        /// An element is contained in this interval if and only if it is
        /// higher than the interval's lower endpoint (or greater than or equal
        /// to if the endpoint is inclusive) and lower than the interval's
        /// upper endpoint (or less than or equal to if the endpoint is
        /// inclusive).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"></see> if and only if this set
        /// contains the given element. <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(T value) => 
            LowerEndpoint <= value && UpperEndpoint >= value;

        /// <summary>
        /// This method creates a new set which contains every element that 
        /// is contained in both this interval and the given interval.
        /// </summary>
        /// <param name="other">The other interval. Cannot be null.</param>
        /// <returns>A set which contains only those elements
        /// which are contained in both this interval and the other given
        /// interval.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public DenseInterval<T> Intersect(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            if (IsSubsetOf(other))
            {
                return other;
            }
            else if (other.IsSubsetOf(this))
            {
                return this;
            }
            else if (other.UpperEndpoint > this.LowerEndpoint &&
                other.UpperEndpoint <= this.UpperEndpoint)
            {
                return new DenseInterval<T>(
                    this.LowerEndpoint, other.UpperEndpoint);
            }
            else if (this.UpperEndpoint > other.LowerEndpoint &&
                this.UpperEndpoint <= other.UpperEndpoint)
            {
                return new DenseInterval<T>(
                    other.LowerEndpoint, this.UpperEndpoint);
            }
            else if (this.LowerEndpoint.IsInclusive)
            {
                return new DenseInterval<T>(
                    LowerEndpoint<T>.Exclusive(this.LowerEndpoint.Value!),
                    UpperEndpoint<T>.Exclusive(this.LowerEndpoint.Value!));
            }
            else
            {
                Debug.Assert(this.UpperEndpoint.IsInclusive);
                return new DenseInterval<T>(
                    LowerEndpoint<T>.Exclusive(this.UpperEndpoint.Value!),
                    UpperEndpoint<T>.Exclusive(this.UpperEndpoint.Value!));
            }
        }

        public bool IsProperSubsetOf(DenseInterval<T> other) =>
            other.LowerEndpoint < LowerEndpoint &&
            other.UpperEndpoint > UpperEndpoint;

        public bool IsSubsetOf(DenseInterval<T> other) =>
            other.LowerEndpoint <= this.LowerEndpoint &&
            other.UpperEndpoint >= this.UpperEndpoint;

        public bool IsSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint >= LowerEndpoint &&
            other.UpperEndpoint <= UpperEndpoint;

        public bool IsProperSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint > LowerEndpoint &&
            other.UpperEndpoint < UpperEndpoint;

        public bool Overlaps(DenseInterval<T> other) =>
            other.UpperEndpoint >= this.LowerEndpoint
            && other.LowerEndpoint <= this.UpperEndpoint;

        public bool IsLower(DenseInterval<T> other) =>
            this.LowerEndpoint < other.LowerEndpoint &&
            this.UpperEndpoint <= other.LowerEndpoint;

        public bool IsUpper(DenseInterval<T> other) =>
            this.LowerEndpoint >= other.UpperEndpoint &&
            this.UpperEndpoint > other.UpperEndpoint;

        public bool IsConnected(DenseInterval<T> other)
        {
            return (other.UpperEndpoint.IsInfinite || this.LowerEndpoint.IsInfinite ||
                other.UpperEndpoint.Value!.CompareTo(this.LowerEndpoint.Value!) >= 0)
                && (other.LowerEndpoint.IsInfinite || this.UpperEndpoint.IsInfinite ||
                other.LowerEndpoint.Value!.CompareTo(this.UpperEndpoint.Value!) <= 0);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is DenseInterval<T> interval
                && LowerEndpoint.Equals(interval.LowerEndpoint)
                && UpperEndpoint.Equals(interval.UpperEndpoint);
        }

        public override int GetHashCode() =>
            HashCode.Combine(LowerEndpoint, UpperEndpoint);

        public override string ToString() =>
            $"{LowerEndpoint}, {UpperEndpoint}";

        public double GetLength(Func<T?, T?, double> measureFunction) => 
            measureFunction(LowerEndpoint.Value, UpperEndpoint.Value);

    }
}
