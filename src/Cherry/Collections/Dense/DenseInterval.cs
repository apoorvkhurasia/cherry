using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

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
    public sealed class DenseInterval<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {

        private readonly ImmutableList<DenseInterval<T>> _disjointRep;

        /// <summary>
        /// Constructs an instance of this class from the given endpoints.
        /// </summary>
        /// <param name="le">The lower endpoint.</param>
        /// <param name="ue">The upper endpoint.</param>
        public DenseInterval(LowerEndpoint<T> le, UpperEndpoint<T> ue)
        {
            LowerEndpoint = le;
            UpperEndpoint = ue;
            _disjointRep = ImmutableList.Create(this);
        }

        /// <summary>
        /// The universe set is a special instance of this class. This set
        /// contains every element of type <typeparamref name="T"/>.
        /// </summary>
        public static DenseInterval<T> UniverseInstance { get; } =
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
        /// <see langword="true"></see> if and only if this set contains 0
        /// elements. <see langword="false"/> otherwise.
        /// </summary>
        public bool IsEmpty => !LowerEndpoint.IsInfinite
            && !UpperEndpoint.IsInfinite
            && Comparer<T>.Default.Compare(
                LowerEndpoint.Value, UpperEndpoint.Value) == 0;

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
        /// <exception cref="ArgumentNullException">The value to
        /// check cannot be null.</exception>
        public bool Contains(T value)
        {
            SE.RequireNonNull(value, nameof(value));
            return LowerEndpoint <= value && UpperEndpoint >= value;
        }

        /// <summary>
        /// This method creates a new set which contains every element
        /// contained in this set and also contains every element in the given
        /// set.
        /// </summary>
        /// <param name="other">The other set.</param>
        /// <returns>The union set which contains all elements of this
        /// set as well as that of the other given set.</returns>
        /// <exception cref="ArgumentNullException">The given set cannot be
        /// null.</exception>
        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));

            var orderedDisjointSets = other.AsDisjointIntervals()
                .Union(this.AsDisjointIntervals())
                .Where(s => !s.IsEmpty)
                .OrderBy(i => i.LowerEndpoint)
                .ThenBy(i => i.UpperEndpoint)
                .ToList();
            if (orderedDisjointSets.Count == 0)
            {
                return EmptySet<T>.Instance;
            }
            else
            {
                for (int i = 1; i < orderedDisjointSets.Count; i++)
                {
                    var preceding = orderedDisjointSets[i - 1];
                    var curr = orderedDisjointSets[i];
                    if (curr.IsConnected(preceding))
                    {
                        orderedDisjointSets[i - 1] = null!;
                        orderedDisjointSets[i] = new DenseInterval<T>(
                            preceding.LowerEndpoint, curr.UpperEndpoint);
                    }
                }
                return orderedDisjointSets.Count == 1 ?
                    orderedDisjointSets[0] :
                    new UnionSet<T>(orderedDisjointSets.Where(i => i != null));
            }
        }

        /// <summary>
        /// This method creates a new set which contains every element that is
        /// contained in both this set and the given set.
        /// </summary>
        /// <param name="other">The other set. Cannot be null.</param>
        /// <returns>The intersection set which contains only those elements
        /// which are contained in both this set and the other given
        /// set.</returns>
        /// <exception cref="ArgumentNullException">The given set cannot
        /// be null.</exception>
        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> other) =>
            new UnionSet<T>(
                this.AsDisjointIntervals()
                    .SelectMany(
                        i => other.AsDisjointIntervals(),
                        (i1, i2) => i1.Intersect(i2))
                    .Where(interval => !interval.IsEmpty));

        /// <summary>
        /// This method creates a new set which contains every element that is
        /// contained in both this interval and the given interval.
        /// </summary>
        /// <param name="other">The other interval. Cannot be null.</param>
        /// <returns>The intersection set which contains only those elements
        /// which are contained in both this interval and the other given
        /// interval.</returns>
        /// <exception cref="ArgumentNullException">The given interval cannot
        /// be null.</exception>
        public IDenseOrderedSet<T> Interset(DenseInterval<T> other)
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
            else
            {
                return EmptySet<T>.Instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsProperSubsetOf);

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsProperSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint < LowerEndpoint &&
            other.UpperEndpoint > UpperEndpoint;

        public bool IsSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsSubsetOf);

        public bool IsSubsetOf(DenseInterval<T> other) =>
            other.LowerEndpoint <= this.LowerEndpoint &&
            other.UpperEndpoint >= this.UpperEndpoint;

        public bool IsSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsSupersetOf(DenseInterval<T> other) =>
            other.LowerEndpoint <= LowerEndpoint &&
            other.UpperEndpoint >= UpperEndpoint;

        public bool Overlaps(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(Overlaps);

        public bool Overlaps(DenseInterval<T> other) =>
            !Intersect(other).IsEmpty;

        public bool IsConnected(DenseInterval<T> other) => 
            UpperEndpoint <= other.LowerEndpoint
                && other.LowerEndpoint <= UpperEndpoint;

        public bool SetEquals(IDenseOrderedSet<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is DenseInterval<T> interval
                && LowerEndpoint.Equals(interval.LowerEndpoint)
                && UpperEndpoint.Equals(interval.UpperEndpoint);
        }

        public IDenseOrderedSet<T> Complement()
        {
            if (ReferenceEquals(this, UniverseInstance))
            {
                return EmptySet<T>.Instance;
            }
            else if (LowerEndpoint.IsInfinite)
            {
                Debug.Assert(UpperEndpoint.Value is not null);
                var le = UpperEndpoint.IsInclusive ?
                    LowerEndpoint<T>.Exclusive(UpperEndpoint.Value) :
                    LowerEndpoint<T>.FiniteInclusive(UpperEndpoint.Value);

                return new DenseInterval<T>(
                    le, UpperEndpoint<T>.PositiveInfinity());
            }
            else if (UpperEndpoint.IsInfinite)
            {
                Debug.Assert(LowerEndpoint.Value is not null);
                var ue = LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.FiniteExclusive(LowerEndpoint.Value) :
                    UpperEndpoint<T>.FiniteInclusive(LowerEndpoint.Value);

                return new DenseInterval<T>(
                    LowerEndpoint<T>.NegativeInfinity(), ue);
            }
            else
            {
                Debug.Assert(UpperEndpoint.Value is not null);
                Debug.Assert(LowerEndpoint.Value is not null);
                var le = UpperEndpoint.IsInclusive ?
                       LowerEndpoint<T>.Exclusive(UpperEndpoint.Value) :
                       LowerEndpoint<T>.FiniteInclusive(UpperEndpoint.Value);
                var ue = LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.FiniteExclusive(LowerEndpoint.Value) :
                    UpperEndpoint<T>.FiniteInclusive(LowerEndpoint.Value);
                return new UnionSet<T>(new[]
                {
                    new DenseInterval<T>(
                        LowerEndpoint<T>.NegativeInfinity(), ue),
                    new DenseInterval<T>(
                        le, UpperEndpoint<T>.PositiveInfinity())
                });
            }
        }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() => 
            _disjointRep;

        public override string ToString() =>
            $"{LowerEndpoint}, {UpperEndpoint}";

    }
}
