using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Cherry.Collections.Dense
{
    public class SetIntersection<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {
        private readonly List<DenseInterval<T>> _disjointIntervalRepresentation;

        public SetIntersection(IEnumerable<IDenseOrderedSet<T>> of)
        {
            var remDisjointIntervals = new List<DenseInterval<T>>();
            foreach (var set in of)
            {
                if (remDisjointIntervals is null)
                {
                    remDisjointIntervals = set.AsDisjointIntervals().ToList();
                }
                else
                {
                    var copy = remDisjointIntervals.ToList();
                    remDisjointIntervals.Clear();
                    foreach (var interval in remDisjointIntervals)
                    {
                        foreach (var dj in set.AsDisjointIntervals())
                        {
                            if (dj.Intersect(interval) is DenseInterval<T> i
                                && !i.IsEmpty)
                            {
                                remDisjointIntervals.Add(i);
                            }
                            else if (dj.LowerEndpoint >= interval.UpperEndpoint)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            _disjointIntervalRepresentation = remDisjointIntervals;
            IsEmpty = _disjointIntervalRepresentation.Count > 0;
        }

        public bool IsEmpty { get; }

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() =>
            _disjointIntervalRepresentation.ToImmutableList();

        public IDenseOrderedSet<T> Complement() => new ComplementSet<T>(this);

        public bool Contains(T item) =>
            _disjointIntervalRepresentation.Any(s => s.Contains(item));

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another) =>
            new SetIntersection<T>(Enumerable.Concat(AsDisjointIntervals(),
                Enumerable.Repeat(another, 1)));

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IDenseOrderedSet<T> other)
        {
            throw new NotImplementedException();
        }

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another)
        {
            throw new NotImplementedException();
        }
    }
}
