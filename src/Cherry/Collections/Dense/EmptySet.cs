using System;
using System.Collections.Immutable;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// A set that contains nothing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EmptySet<T> : IDenseOrderedSet<T> 
        where T : IComparable<T>
    {
        private static readonly string STR_REP = "∅";

        private EmptySet() {  }

        /// <summary>
        /// The one and only instance of this class.
        /// </summary>
        public static EmptySet<T> Instance { get; } = new();

        /// <summary>
        /// Returns <see langword="true"/>.
        /// </summary>
        public bool IsEmpty => true;

        /// <summary>
        /// Returns an empty list.
        /// </summary>
        public ImmutableList<DenseInterval<T>> AsDisjointIntervals() =>
            ImmutableList<DenseInterval<T>>.Empty;

        /// <summary>
        /// Returns a set that contains all values of type 
        /// <typeparamref name="T"/>. Callers should not make specific
        /// assumptions about the type of the returned object. However,
        /// they can rely on the fact that calling
        /// <see cref="IDenseOrderedSet{T}.Complement()"/> on the returned 
        /// object will return an <see cref="EmptySet{T}"/> .
        /// </summary>
        /// <returns>The universe set.</returns>
        public IDenseOrderedSet<T> Complement() =>
            DenseInterval<T>.UniverseInstance;

        /// <summary>
        /// Returns <see langword="false" /> always.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="false"/>.</returns>
        public bool Contains(T item) => false;

        /// <summary>
        /// Returns an <see cref="EmptySet{T}"/> always.
        /// </summary>
        /// <param name="another">The set to intersect this set with.</param>
        /// <returns>An instance of <see cref="EmptySet{T}"/>.</returns>
        public IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another) 
            => this;

        /// <summary>
        /// An empty set is a proper subset of every set except the empty
        /// set itself. Returns <see langword="true"/> if and only if
        /// the set to check is not empty. <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns><see langword="true"/> if and only if
        /// the set to check is not empty. <see langword="false"/> otherwise.
        /// </returns>
        public bool IsProperSubsetOf(IDenseOrderedSet<T> other) => 
            !other.IsEmpty;

        /// <summary>
        /// Always returns <see langword="false"/>. An empty set is not
        /// a proper superset of any set.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns><see langword="false"/>.</returns>
        public bool IsProperSupersetOf(IDenseOrderedSet<T> other) => false;

        /// <summary>
        /// An empty set is subset of every set. This method always returns
        /// <see langword="true"/>.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns><see langword="true"/>.</returns>
        public bool IsSubsetOf(IDenseOrderedSet<T> other) => true;

        /// <summary>
        /// An empty set is not a superset of any set except the empty
        /// set itself. This method returns <see langword="true"/> if
        /// and only if the given set is an empty set. <see langword="false"/>
        /// otherwise.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns></returns>
        public bool IsSupersetOf(IDenseOrderedSet<T> other) => false;

        /// <summary>
        /// An empty set overlaps with no other set. This method always
        /// returns <see langword="false"/>.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns><see langword="false"/>.</returns>
        public bool Overlaps(IDenseOrderedSet<T> other) => false;

        /// <summary>
        /// The empty set is only equal to a set which itself is empty. This
        /// method returns <see langword="true"/> if and only if 
        /// <paramref name="other"/> is empty. <see langword="false"/> 
        /// otherwise.
        /// </summary>
        /// <param name="other">The set to check.</param>
        /// <returns><see langword="true"/> if and only if the given
        /// set is empty. <see langword="false"/> otherwise.</returns>
        public bool SetEquals(IDenseOrderedSet<T> other) => other.IsEmpty;

        /// <summary>
        /// The union of the given set with this set.
        /// </summary>
        /// <param name="another">The set with which union has to be taken.
        /// </param>
        /// <returns>A set which is equal (in the sense of
        /// <see cref="IDenseOrderedSet{T}.SetEquals(IDenseOrderedSet{T})"/>)
        /// to the given set.</returns>
        public IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another) 
            => another;

        public override bool Equals(object? obj) => 
            obj is EmptySet<T> e && Equals(e);

        public override int GetHashCode() => 17;

        public override string ToString() => STR_REP;
    }
}