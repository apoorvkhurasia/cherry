using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using SE = Cherry.StandardExceptions;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// A set which is the union of intervals.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DenseOrderedSet<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {
        private readonly ImmutableList<DenseInterval<T>> _disjointIntervals;
        private int _memoizedHashCode = 0;
        private string? _stringRepresentation;

        private readonly object _hashCodeLock = new();
        private readonly object _stringRepLock = new();

        #region Factory Methods

        /// <summary>
        /// Creates a <see cref="DenseOrderedSet{T}"/> from a single interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>A <see cref="DenseOrderedSet{T}"/> created from the given
        /// interval.</returns>
        /// <exception cref="ArgumentNullException">The given interval
        /// cannot be null.</exception>
        public static DenseOrderedSet<T> FromInterval(DenseInterval<T> interval)
        {
            SE.RequireNonNull(interval, nameof(interval));
            if (interval.IsEmpty)
            {
                return DenseOrderedSet<T>.Empty;
            }
            else if (interval.IsUniverse)
            {
                return DenseOrderedSet<T>.Universe;
            }
            else
            {
                return new(Enumerable.Repeat(interval, 1));
            }
        }

        /// <summary>
        /// Creates a <see cref="DenseOrderedSet{T}"/> from the complement
        /// of the given interval.
        /// </summary>
        /// <param name="interval">The given interval.</param>
        /// <returns>A <see cref="DenseOrderedSet{T}"/> from the complement
        /// of the given interval. </returns>
        /// <exception cref="ArgumentNullException">The given interval
        /// cannot be null.</exception>
        public static DenseOrderedSet<T> AsComplementOf(
            DenseInterval<T> interval)           
        {
            SE.RequireNonNull(interval, nameof(interval));
            if (interval.IsUniverse)
            {
                return DenseOrderedSet<T>.Empty;
            }
            else if (interval.IsEmpty)
            {
                return DenseOrderedSet<T>.Universe;
            }
            else if (interval.LowerEndpoint.IsInfinite)
            {
                var le = interval.UpperEndpoint.IsInclusive ?
                    LowerEndpoint<T>.Exclusive(interval.UpperEndpoint.Value!) :
                    LowerEndpoint<T>.Inclusive(interval.UpperEndpoint.Value!);

                return FromInterval(new DenseInterval<T>(
                    le, UpperEndpoint<T>.PositiveInfinity()));
            }
            else if (interval.UpperEndpoint.IsInfinite)
            {
                var ue = interval.LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.Exclusive(interval.LowerEndpoint.Value!) :
                    UpperEndpoint<T>.Inclusive(interval.LowerEndpoint.Value!);

                return FromInterval(new DenseInterval<T>(
                    LowerEndpoint<T>.NegativeInfinity(), ue));
            }
            else
            {
                var le = interval.UpperEndpoint.IsInclusive ?
                    LowerEndpoint<T>.Exclusive(interval.UpperEndpoint.Value!) :
                    LowerEndpoint<T>.Inclusive(interval.UpperEndpoint.Value!);
                var ue = interval.LowerEndpoint.IsInclusive ?
                    UpperEndpoint<T>.Exclusive(interval.LowerEndpoint.Value!) :
                    UpperEndpoint<T>.Inclusive(interval.LowerEndpoint.Value!);
                return new(new[]
                {
                    new DenseInterval<T>(
                        LowerEndpoint<T>.NegativeInfinity(), ue),
                    new DenseInterval<T>(
                        le, UpperEndpoint<T>.PositiveInfinity())
                });
            }
        }

        /// <summary>
        /// The empty set which contains nothing.
        /// </summary>
        public static DenseOrderedSet<T> Empty { get; }
            = new(Array.Empty<DenseInterval<T>>());

        /// <summary>
        /// The universe set which contains all elements of type
        /// <typeparamref name="T"/>.
        /// </summary>
        public static DenseOrderedSet<T> Universe { get; }
            = new(Enumerable.Repeat(DenseInterval<T>.Universe, 1));

        /// <summary>
        /// Creates a <see cref="DenseOrderedSet{T}"/> from the given intervals.
        /// Any and all connected intervals will be collapsed into a single
        /// interval spanning them.
        /// </summary>
        /// <param name="of">The intervals.</param>
        /// <returns>A <see cref="DenseOrderedSet{T}"/> created from the given
        /// intervals.</returns>
        /// <exception cref="ArgumentNullException">The given interval
        /// enumberable cannot be null.</exception>
        public static DenseOrderedSet<T> Union(IEnumerable<DenseInterval<T>> of)
        {
            SE.RequireNonNull(of, nameof(of));
            List<DenseInterval<T>> nonEmpty = new();
            foreach (var interval in of)
            {
                if (interval.IsUniverse)
                {
                    return DenseOrderedSet<T>.Universe;
                }
                else if (!interval.IsEmpty)
                {
                    nonEmpty.Add(interval);
                }
            }
            return new(nonEmpty);
        }

        /// <summary>
        /// Creates a <see cref="DenseOrderedSet{T}"/> from the intersection
        /// of the given intervals.
        /// </summary>
        /// <param name="of">The intervals.</param>
        /// <returns>A <see cref="DenseOrderedSet{T}"/> created from the
        /// intersection of the given intervals.</returns>
        /// <exception cref="ArgumentNullException">The given interval
        /// enumberable cannot be null.</exception>
        public static DenseOrderedSet<T> Intersection(
            IEnumerable<DenseInterval<T>> of)
        {
            var intersection = DenseInterval<T>.Universe;
            foreach (var interval in of)
            {
                if (interval.IsEmpty || intersection.IsEmpty)
                {
                    return Empty;
                }
                intersection = intersection.Intersect(interval);
            }
            return FromInterval(intersection);
        }

        #endregion

        private DenseOrderedSet(IEnumerable<DenseInterval<T>> of)
        {
            var orderedIntervals = of.Where(s => !s.IsEmpty)
                .OrderBy(i => i.LowerEndpoint)
                .ThenBy(i => i.UpperEndpoint)
                .ToList();
            for (int i = 1; i < orderedIntervals.Count; i++)
            {
                var preceding = orderedIntervals[i - 1];
                var curr = orderedIntervals[i];
                if (curr.IsConnected(preceding))
                {
                    orderedIntervals[i - 1] = null!;
                    orderedIntervals[i] = new DenseInterval<T>(
                        preceding.LowerEndpoint, curr.UpperEndpoint);
                }
            }

            _disjointIntervals = orderedIntervals
                .Where(s => s is not null).ToImmutableList();
            IsEmpty = !_disjointIntervals.Any(i => !i.IsEmpty);
        }

        public bool IsEmpty { get; }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() =>
            _disjointIntervals;

        public IDenseOrderedSet<T> Complement()
        {
            if (IsEmpty)
            {
                return Universe;
            }
            var myIntervals = 
                new Queue<DenseInterval<T>>(AsDisjointIntervals());
            var complementIntervals = 
                new List<DenseInterval<T>>(myIntervals.Count + 1);
            
            while (myIntervals.TryDequeue(out var interval))
            {
                if (interval.IsUniverse)
                {
                    return Empty;
                }

                if (!interval.LowerEndpoint.IsInfinite)
                {
                    var start = LowerEndpoint<T>.NegativeInfinity();
                    var endVal = interval.LowerEndpoint.Value!;
                    var end = interval.LowerEndpoint.IsInclusive ?
                        UpperEndpoint<T>.Exclusive(endVal) :
                        UpperEndpoint<T>.Inclusive(endVal);
                    complementIntervals.Add(new DenseInterval<T>(start, end));
                }

                var nextStartVal = interval.UpperEndpoint.Value!;
                var nextStart = interval.UpperEndpoint.IsInclusive ?
                    LowerEndpoint<T>.Exclusive(nextStartVal) :
                    LowerEndpoint<T>.Inclusive(nextStartVal);

                if (myIntervals.TryPeek(out var nextInterval))
                {
                    var endVal = nextInterval.LowerEndpoint.Value!;
                    var end = nextInterval.LowerEndpoint.IsInclusive ?
                        UpperEndpoint<T>.Exclusive(endVal) :
                        UpperEndpoint<T>.Inclusive(endVal);
                    complementIntervals.Add(
                        new DenseInterval<T>(nextStart, end));
                }
                else
                {
                    complementIntervals.Add(
                        new DenseInterval<T>(nextStart,
                            UpperEndpoint<T>.PositiveInfinity()));
                }
            }
            return Union(complementIntervals);
        }

        public bool Contains(T item)
        {
            foreach (var interval in AsDisjointIntervals())
            {
                var pos = interval.GetPosition(item);
                if (pos == PointPosition.LOWER)
                {
                    return false; //No point examining other intervals
                }
                else if (pos != PointPosition.UPPER)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns another set which contains elements of this set and the
        /// given set.
        /// </summary>
        /// <param name="other">Another set.</param>
        /// <returns>A set which contains elements of this set and the
        /// given set.</returns>
        /// <exception cref="ArgumentNullException">The given set cannot be
        /// null.</exception>
        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return Union(
                Enumerable.Concat(AsDisjointIntervals(), 
                other.AsDisjointIntervals()));
        }

        /// <summary>
		/// Returns another set which contains elements belonging to
		/// both this set and the given set.
		/// </summary>
		/// <param name="another">Another set.</param>
		/// <returns>A set which contains elements belonging to
		/// both this set and the given set.</returns>
        /// <exception cref="ArgumentNullException">The given set cannot be
        /// null.</exception>
        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another)
        {
            SE.RequireNonNull(another, nameof(another));
            var remDisjointIntervals = new List<DenseInterval<T>>();
            foreach (var interval in AsDisjointIntervals())
            {
                foreach (var dj in another.AsDisjointIntervals())
                {
                    var intersection = dj.Intersect(interval);
                    if (!intersection.IsEmpty)
                    {
                        remDisjointIntervals.Add(intersection);
                    }
                    else if (dj.LowerEndpoint >= interval.UpperEndpoint)
                    {
                        break;
                    }
                }
            }
            return new DenseOrderedSet<T>(remDisjointIntervals);
        }

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.AsDisjointIntervals().All(dji =>
                this.AsDisjointIntervals().Any(
                    thisDji => dji.IsProperSubsetOf(thisDji)
                )
            );
        }

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.IsProperSubsetOf(this);
        }

        public bool IsSubsetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return other.AsDisjointIntervals().All(dji =>
                this.AsDisjointIntervals().Any(
                    thisDji => dji.IsSubsetOf(thisDji)
                )
            );
        }

        public bool IsSupersetOf(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            var otherIntervals = other.AsDisjointIntervals();
            return this.AsDisjointIntervals().All(dji =>
                otherIntervals.Any(o => dji.IsSubsetOf(o)));
        }

        public bool Overlaps(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            var myIntervals = AsDisjointIntervals();
            var theirIntervals = other.AsDisjointIntervals();
            for (int i = 0; i < myIntervals.Count; )
            {
                var mine = myIntervals[i];
                for (int j = 0; j < theirIntervals.Count; )
                {
                    var their = theirIntervals[j];
                    if (mine.Overlaps(their))
                    {
                        return true;
                    }
                    else if (mine.IsLower(their))
                    {
                        i++;
                    }
                    else
                    {
                        Debug.Assert(their.IsLower(mine));
                        j++;
                    }
                }
            }
            return false;
        }

        public bool SetEquals(IDenseOrderedSet<T> other) 
        {
            if (other is null)
            {
                return false;
            }
            else if (ReferenceEquals(this, other))
            {
                return true;
            }

            var myIntervals = AsDisjointIntervals();
            var theirIntervals = other.AsDisjointIntervals();
            if (myIntervals.Count != theirIntervals.Count)
            {
                return false;
            }

            for (int i = 0; i < myIntervals.Count; i++)
            {
                if (!myIntervals[i].Equals(theirIntervals[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (_memoizedHashCode == 0)
            {
                lock (_hashCodeLock)
                {
                    var hashCode = new HashCode();
                    _disjointIntervals.ForEach(
                        interval => hashCode.Add(interval.GetHashCode()));
                    _memoizedHashCode = hashCode.ToHashCode();
                }
            }
            return _memoizedHashCode;
        }

        /// <summary>
        /// An object is equal to this instance if and only if it is a
        /// <see cref="DenseOrderedSet{T}"/> containing all elements of this set
        /// and if this set contains all elements contained in the given object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><see langword="true" /> if and only if the given
        /// object is a <see cref="DenseOrderedSet{T}"/> equal in the
        /// mathematical sense to this set. <see langword="false" /> otherwise.
        /// </returns>
        public override bool Equals(object? obj) => 
            obj is DenseOrderedSet<T> set && SetEquals(set);

        /// <summary>
        /// Returns a string representation of this set in the standard
        /// mathematical notation.
        /// </summary>
        /// <returns>The string representation of this set in the standard
        /// mathematical notation.</returns>
        public override string ToString()
        {
            if (_stringRepresentation == null)
            {
                lock (_stringRepLock)
                {
                    _stringRepresentation =
                        string.Join(" ∪ ", _disjointIntervals);
                }
            }
            return _stringRepresentation;
        }

        /// <summary>
        /// Gets the length of this set using the given measure.
        /// </summary>
        /// <param name="measureFunction">The measure. This function
        /// should be able to handle infinities as configured in 
        /// <see cref="TypeConfiguration"/>.</param>
        /// <returns>The length of this set.</returns>
        /// <exception cref="ArgumentNullException">The measure function
        /// cannot be null.</exception>
        public double GetLength(Func<T?, T?, double> measureFunction) => 
            _disjointIntervals.Sum(i => i.GetLength(measureFunction));

        public IEnumerable<T> Sample(Func<T, T> generator)
        {
            if (IsEmpty)
            {
                yield break;
            }

            var intervals = AsDisjointIntervals();
            var start = intervals.First().LowerEndpoint;
            var end = intervals.Last().UpperEndpoint;
            T item;
            if (!start.IsInfinite ||
                !TypeConfiguration.TryGetNegativeInfinity(out item))
            {
                item = start.Value!;
            }
            while (item <= end)
            {
                if (Contains(item))
                {
                    yield return item;
                }
                item = generator(item);
            }
        }
    }
}
