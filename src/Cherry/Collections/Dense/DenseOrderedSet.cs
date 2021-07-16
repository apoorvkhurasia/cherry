using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        #region Factory Methods

        public static DenseOrderedSet<T> FromInterval(DenseInterval<T> interval)
        {
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

        public static DenseOrderedSet<T> AsComplementOf(
            DenseInterval<T> interval)           
        {
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

        public static DenseOrderedSet<T> Empty { get; }
            = new(Array.Empty<DenseInterval<T>>());

        public static DenseOrderedSet<T> Universe { get; }
            = new(Enumerable.Repeat(DenseInterval<T>.Universe, 1));

        public static DenseOrderedSet<T> Union(IEnumerable<DenseInterval<T>> of)
        {
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

        public static DenseOrderedSet<T> Intersection(
            IEnumerable<DenseInterval<T>> of)
        {
            List<DenseInterval<T>> nonEmpty = new();
            foreach (var interval in of)
            {
                if (interval.IsEmpty)
                {
                    return DenseOrderedSet<T>.Empty;
                }
                else if (!interval.IsUniverse)
                {
                    nonEmpty.AddRange(
                        AsComplementOf(interval).AsDisjointIntervals());
                }
            }
            return new(nonEmpty);
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

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other)
        {
            SE.RequireNonNull(other, nameof(other));
            return Union(
                Enumerable.Concat(AsDisjointIntervals(), 
                other.AsDisjointIntervals()));
        }

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
            foreach(var mine in AsDisjointIntervals())
            {
                foreach(var their in other.AsDisjointIntervals())
                {
                    if (mine.IsUpper(their))
                    {
                        break; //No point going further
                    }
                    else if (mine.Overlaps(their))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetEquals(IDenseOrderedSet<T> other) 
        {
            SE.RequireNonNull(other, nameof(other));
            return IsSubsetOf(other) && other.IsSubsetOf(this);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            _disjointIntervals.ForEach(hc.Add);
            return hc.ToHashCode();
        }

        public override bool Equals(object? obj) => 
            obj is DenseOrderedSet<T> set && SetEquals(set);

        public override string ToString()
        {
            return string.Join(" ∪ ", _disjointIntervals);
        }

        public double GetLength(Func<T?, T?, double> measureFunction) => 
            _disjointIntervals.Sum(i => i.GetLength(measureFunction));

    }
}
