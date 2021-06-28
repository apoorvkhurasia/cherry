using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace  Cherry.Collections.Dense
{
    public class LazyUnion<T> : IDenseOrderedSet<T> where T : IComparable<T>
    {
        private readonly IEnumerable<IDenseOrderedSet<T>> _underlyingSets;

        internal LazyUnion(IEnumerable<IDenseOrderedSet<T>> of)
            => _underlyingSets = of;

        public bool IsEmpty
            => _underlyingSets.All(s => s.IsEmpty);

        public ImmutableList<Interval<T>> AsDisjointIntervals()
        {
            var allIntervals = _underlyingSets
                .SelectMany(s => s.AsDisjointIntervals())
                .OrderBy(s => s.LowerEndpoint)
                .ToList();

            throw new NotImplementedException();
        }

        public IDenseOrderedSet<T> Complement()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another)
        {
            throw new NotImplementedException();
        }

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
