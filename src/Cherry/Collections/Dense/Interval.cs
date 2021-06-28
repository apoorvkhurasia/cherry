using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace  Cherry.Collections.Dense
{
    public sealed class Interval<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {
        private readonly ImmutableList<Interval<T>> _disjointRep;

        internal Interval(LowerEndpoint<T> lowerEndpoint,
            UpperEndpoint<T> upperEndpoint)
        {
            _disjointRep = ImmutableList.Create(this);
        }

        public LowerEndpoint<T> LowerEndpoint { get; }

        public UpperEndpoint<T> UpperEndpoint { get; }

        public bool IsEmpty => LowerEndpoint.IsInclusive
            && UpperEndpoint.IsInclusive
            && Comparer<T>.Default.Compare(
                LowerEndpoint.Value, UpperEndpoint.Value) == 0;

        public bool Contains(T value) =>
            LowerEndpoint <= value && UpperEndpoint >= value;

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another)
        {
            //TODO implement
        }

        public IDenseOrderedSet<T> Union(Interval<T> another)
        {
            if (LowerEndpoint.Equals(another.UpperEndpoint))
            {
                return new Interval<T>(another.LowerEndpoint, UpperEndpoint);
            }
            else if (another.LowerEndpoint.Equals(UpperEndpoint))
            {
                return new Interval<T>(LowerEndpoint, another.UpperEndpoint);
            }
            else if (IsSubsetOf(another))
            {
                return another;
            }
            else if (another.IsSubsetOf(this))
            {
                return this;
            }
            else
            {
                return new LazyUnion<T>(new[] { this, another });
            }
        }

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another)
        {
            throw new NotImplementedException();
        }

        public IDenseOrderedSet<T> Interset(Interval<T> another)
        {
            if (IsSubsetOf(another))
            {
                return another;
            }
            else if (another.IsSubsetOf(this))
            {
                return this;
            }
            else if (!Overlaps(this))
            {
                return new EmptySet<T>();
            }
            else
            {
                return new LazyIntersection<T>(new[] { this, another });
            }
        }

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsProperSubsetOf);

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsProperSupersetOf(Interval<T> other) =>
            other.LowerEndpoint < LowerEndpoint &&
            other.UpperEndpoint > UpperEndpoint;

        public bool IsSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(IsSubsetOf);

        public bool IsSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(IsProperSupersetOf);

        public bool IsSupersetOf(Interval<T> other) =>
            other.LowerEndpoint <= LowerEndpoint &&
            other.UpperEndpoint >= UpperEndpoint;

        public bool Overlaps(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().Any(Overlaps);

        public bool Overlaps(Interval<T> other) =>
            IsSubsetOf(other) || other.IsSubsetOf(this);

        public bool SetEquals(IDenseOrderedSet<T> other) =>
            IsSubsetOf(other) && other.IsSubsetOf(this);

        public IDenseOrderedSet<T> Complement()
        {
            return new ComplementSet<T>(this);
        }

        public ImmutableList<Interval<T>> AsDisjointIntervals() =>
            _disjointRep;
    }
}
