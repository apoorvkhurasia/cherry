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

        private UnionSet(IList<DenseInterval<T>> of)
        {
            _disjointIntervalRep = of.ToImmutableList();
            IsEmpty = _disjointIntervalRep.Count > 0;
        }

        public static IDenseOrderedSet<T> Of(
            IEnumerable<IDenseOrderedSet<T>> sets)
        {
            var orderedDisjointSets =
                sets.SelectMany(s => s.AsDisjointIntervals())
                .Where(s => !s.IsEmpty)
                .OrderBy(i => i.LowerEndpoint)
                .ThenBy(i => i.UpperEndpoint)
                .ToList();
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

            var disconnectedSets = orderedDisjointSets
                .Where(s => s is not null).ToList();
            if (disconnectedSets.Count == 0)
            {
                return EmptySet<T>.Instance;
            }
            else if (disconnectedSets.Count == 1)
            {
                return disconnectedSets[0];
            }
            else
            {
                return new UnionSet<T>(disconnectedSets);
            }
        }

        public bool IsEmpty { get; }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() => 
            _disjointIntervalRep;

        public IDenseOrderedSet<T> Complement() => new ComplementSet<T>(this);

        public bool Contains(T item) =>
            _disjointIntervalRep.Any(s => s.Contains(item));

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other) => 
            UnionSet<T>.Of(new[] { this, other });

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another)
        {

        }

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
