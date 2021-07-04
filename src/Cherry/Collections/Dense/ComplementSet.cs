using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace  Cherry.Collections.Dense
{
    public class ComplementSet<T> : IDenseOrderedSet<T> where T : IComparable<T>
    {
        private readonly IDenseOrderedSet<T> _complementOf;

        public ComplementSet(IDenseOrderedSet<T> of)
        {
            _complementOf = of;
        }

        public bool IsEmpty => !_complementOf.IsEmpty;

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() =>
            new ComplementSet<T>(
                new UnionSet<T>(_complementOf.AsDisjointIntervals()))
            .AsDisjointIntervals();

        public IDenseOrderedSet<T> Complement() => _complementOf;

        public bool Contains(T item) => !_complementOf.Contains(item);

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
