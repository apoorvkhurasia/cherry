using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Cherry.Collections.Dense
{
    internal sealed class UnionSet<T>
        : IDenseOrderedSet<T> where T : IComparable<T>
    {
        private readonly ImmutableList<DenseInterval<T>> _disjointIntervalRep;

        internal UnionSet(IEnumerable<IDenseOrderedSet<T>> of)
        {
            _disjointIntervalRep = SetMath.Union(of).ToImmutableList();
            IsEmpty = _disjointIntervalRep.Count > 0;
        }

        public bool IsEmpty { get; }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() => 
            _disjointIntervalRep;

        public IDenseOrderedSet<T> Complement() => new ComplementSet<T>(this);

        public bool Contains(T item) =>
            _disjointIntervalRep.Any(s => s.Contains(item));

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another) =>
            new SetIntersection<T>(Enumerable.Concat(_disjointIntervalRep,
                Enumerable.Repeat(another, 1)));

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(dji =>
                this._disjointIntervalRep.Any(
                    thisDji => dji.IsProperSubsetOf(thisDji)
                )
            );

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(dji =>
                this._disjointIntervalRep.Any(
                    thisDji => dji.IsProperSubsetOf(thisDji)
                )
            );

        public bool IsSubsetOf(IDenseOrderedSet<T> other) =>
            other.AsDisjointIntervals().All(dji =>
                this._disjointIntervalRep.Any(
                    thisDji => dji.IsSubsetOf(thisDji)
                )
            );

        public bool IsSupersetOf(IDenseOrderedSet<T> other)
        {
            var otherIntervals = other.AsDisjointIntervals(); 
            return this.AsDisjointIntervals().All(dji =>
             otherIntervals.Any(o => dji.IsSubsetOf(o)));
        }

        public bool Overlaps(IDenseOrderedSet<T> other) =>
            _disjointIntervalRep.Any(i => i.Overlaps(other));

        public bool SetEquals(IDenseOrderedSet<T> other) =>
            IsSubsetOf(other) && other.IsSubsetOf(this);

    }
}
