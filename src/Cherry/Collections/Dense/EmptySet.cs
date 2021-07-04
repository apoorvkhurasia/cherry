using System;
using System.Collections.Immutable;

namespace Cherry.Collections.Dense
{
    internal class EmptySet<T> : IDenseOrderedSet<T> 
        where T : IComparable<T>
    {
        private EmptySet() {  }

        public static EmptySet<T> Instance { get; } = new();

        public bool IsEmpty => true;

        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() =>
            ImmutableList<DenseInterval<T>>.Empty;

        public IDenseOrderedSet<T> Complement() => new ComplementSet<T>(this);

        public bool Contains(T item) => false;

        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another) 
            => this;

        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) => true;

        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) => false;

        public bool IsSubsetOf(IDenseOrderedSet<T> other) => true;

        public bool IsSupersetOf(IDenseOrderedSet<T> other) => false;

        public bool Overlaps(IDenseOrderedSet<T> other) => true;

        public bool SetEquals(IDenseOrderedSet<T> other) => other.IsEmpty;

        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another) => another;
    }
}