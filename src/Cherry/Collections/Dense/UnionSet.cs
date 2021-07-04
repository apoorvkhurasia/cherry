using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Cherry.Collections.Dense
{
    public sealed class UnionSet<T>
        : IDenseOrderedSet<T> where T : IComparable<T>
    {
        private readonly ImmutableList<DenseInterval<T>> _disjointIntervalRep;

        public UnionSet(IEnumerable<IDenseOrderedSet<T>> of)
        {
            var orderedDisjointSets = of
                .SelectMany(s => s.AsDisjointIntervals())
                .Where(s => !s.IsEmpty)
                .OrderBy(i => i.LowerEndpoint)
                .ThenBy(i => i.UpperEndpoint)
                .ToList();
            if (orderedDisjointSets.Count == 0)
            {
                _disjointIntervalRep = ImmutableList<DenseInterval<T>>.Empty;
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
                _disjointIntervalRep = orderedDisjointSets
                    .Where(i => i is not null).ToImmutableList();
            }           
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

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another) =>
            new UnionSet<T>(Enumerable.Concat(_disjointIntervalRep, 
                Enumerable.Repeat(another, 1)));
    }
}
