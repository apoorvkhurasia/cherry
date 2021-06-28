using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Cherry.Collections.Dense
{
    public class LazyIntersection<T> : IDenseOrderedSet<T>
        where T : IComparable<T>
    {
        private readonly IEnumerable<IDenseOrderedSet<T>> _underlyingSets;

        public LazyIntersection(IEnumerable<IDenseOrderedSet<T>> of)
            => _underlyingSets = of;

        public IComparer<T> ValueComparer =>
            _underlyingSets.Select(s => s.ValueComparer).First();

        public bool IsEmpty => throw new NotImplementedException();

        public ImmutableList<Interval<T>> AsDisjointIntervals()
        {
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
