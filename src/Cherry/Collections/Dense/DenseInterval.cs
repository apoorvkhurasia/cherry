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
                return this;
            }
            else if (other.IsSubsetOf(this))
            {
                return other;
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
            else if (!this.LowerEndpoint.IsInfinite)
            {
                return new DenseInterval<T>(
                    LowerEndpoint<T>.Exclusive(this.LowerEndpoint.Value!),
                    UpperEndpoint<T>.Exclusive(this.LowerEndpoint.Value!));
            }
            else
            {
                Debug.Assert(!this.UpperEndpoint.IsInfinite);
                return new DenseInterval<T>(
                    LowerEndpoint<T>.Exclusive(this.UpperEndpoint.Value!),
                    UpperEndpoint<T>.Exclusive(this.UpperEndpoint.Value!));
            }
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if this
        /// interval is fully contained in the given interval. This means
        /// that all the points in this interval lie in the given
        /// interval but there are points in the given interval which do
        /// not lie in this interval. <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if this
        /// interval is fully contained in the given interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsProperSubsetOf(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.LowerEndpoint < LowerEndpoint
                && other.UpperEndpoint > UpperEndpoint;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if the given
        /// interval is fully contained in this interval. This means
        /// that all the points in the given interval lie in this
        /// interval but there are points in this interval which do
        /// not lie in the given interval. <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if the
        /// given interval is fully contained in this interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsProperSupersetOf(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.IsProperSubsetOf(this);
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if all the points
        /// in this interval lie in the given interval.
        /// <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if all
        /// the points in this interval lie in the given interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsSubsetOf(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.LowerEndpoint <= this.LowerEndpoint
                && other.UpperEndpoint >= this.UpperEndpoint;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if all the points
        /// in the given interval lie in this interval.
        /// <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if all the
        /// points in the given interval lie in this interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsSupersetOf(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.IsSubsetOf(this);
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if there
        /// is at least one point which lies in this as well as the
        /// given interval. <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if there
        /// is at least one point which lies in this as well as the
        /// given interval. <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool Overlaps(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.UpperEndpoint >= this.LowerEndpoint
                && other.LowerEndpoint <= this.UpperEndpoint;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if all
        /// the points in this interval are less than every
        /// point in the given interval. <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if all
        /// the points in this interval are less than every
        /// point in the given interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsLower(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return this.LowerEndpoint < other.LowerEndpoint
                && this.UpperEndpoint < other.LowerEndpoint;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if all
        /// the points in this interval are greater than every
        /// point in the given interval. <see langword="false" /> otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if all
        /// the points in this interval are greater than every
        /// point in the given interval.
        /// <see langword="false" /> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsUpper(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return this.LowerEndpoint > other.UpperEndpoint
                && this.UpperEndpoint > other.UpperEndpoint;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if the union of two
        /// intervals itself is an interval. <see langword="false" />
        /// otherwise.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>Returns <see langword="true" /> if and only if the union of
        /// two intervals itself is an interval. <see langword="false" />
        /// otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public bool IsConnected(DenseInterval<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            var first = other;
            var second = this;
            if (this.LowerEndpoint < other.LowerEndpoint)
            {
                first = this;
                second = other;
            }

            if (first.UpperEndpoint.IsInfinite ||
                second.LowerEndpoint.IsInfinite)
            {
                return true;
            }
            else if (first.UpperEndpoint.IsInclusive ||
                     second.LowerEndpoint.IsInclusive)
            {
                return first.UpperEndpoint.Value!
                    .CompareTo(second.LowerEndpoint.Value) >= 0;
            }
            else
            {
                return first.UpperEndpoint.Value!
                    .CompareTo(second.LowerEndpoint.Value) > 0;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if and only if the given object
        /// is a <see cref="DenseInterval{T}"/> with the same endpoints
        /// as this interval. <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><see langword="true"/> if and only if the given object
        /// is a <see cref="DenseInterval{T}"/> with the same endpoints
        /// as this interval. <see langword="false"/> otherwise.</returns>
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

        /// <summary>
        /// A string representation of this interval in the standard
        /// mathematical notation.
        /// </summary>
        /// <returns>A string representation of this interval in the standard
        /// mathematical notation.</returns>
        public override string ToString() =>
            $"{LowerEndpoint}, {UpperEndpoint}";

        /// <summary>
        /// Gets the length of this interval using the given measure.
        /// </summary>
        /// <param name="measureFunction">The measure. This function
        /// must be able to handle infinities if those are defined
        /// for the <typeparamref name="T"/> in
        /// <see cref="TypeConfiguration"/></param>
        /// <returns>The length of this interval as measured
        /// by the given function.</returns>
        public double GetLength(Func<T?, T?, double> measureFunction) =>
            measureFunction(LowerEndpoint.Value, UpperEndpoint.Value);
    }
}