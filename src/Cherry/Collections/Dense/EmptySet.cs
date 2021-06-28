using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cherry.Math
{
    internal class EmptyUncountableSet<T> : IUncountableSet<T>
    {
        public EmptyUncountableSet() : this(Comparer<T>.Default) { }

        public EmptyUncountableSet(Comparison<T> cmp)
            : this(Comparer<T>.Create(cmp)) { }

        public EmptyUncountableSet(IComparer<T> valueComparer)
        {
            ValueComparer = valueComparer;
        }

        public IComparer<T> ValueComparer { get; }

        public bool IsEmpty => true;

        public ImmutableList<Interval<T>> AsDisjointIntervals() =>
            ImmutableList<Interval<T>>.Empty;

        public IUncountableSet<T> Complement()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item) => false;

        public IUncountableSet<T> Intersect(IUncountableSet<T> another) => this;

        public bool IsProperSubsetOf(IUncountableSet<T> other) => true;

        public bool IsProperSupersetOf(IUncountableSet<T> other) => false;

        public bool IsSubsetOf(IUncountableSet<T> other) => true;

        public bool IsSupersetOf(IUncountableSet<T> other) => false;

        public bool Overlaps(IUncountableSet<T> other) => true;

        public bool SetEquals(IUncountableSet<T> other) => other.IsEmpty;

        public IUncountableSet<T> Union(IUncountableSet<T> another) => another;
    }
}