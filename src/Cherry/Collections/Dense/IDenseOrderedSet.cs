using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace  Cherry.Collections.Dense
{
	/// <summary>
	/// Represents a set that holds uncountably many items. The items in the
    /// set must implement <see cref="IComparable{T}"/> and must be dense in the
    /// mathematical sense. The latter requirement means that between any two
    /// items there be an uncountably infinite number of more items of the same
    /// type. Integers are not dense but real numbers and even rational
    /// numbers are dense.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDenseOrderedSet<T> where T : IComparable<T>
    {
		/// <summary>
		/// <see langword="true" /> if and only if this set contains no
        /// elements. <see langword="false""/> otherwise.
		/// </summary>
		bool IsEmpty { get; }

        /// <summary>Determines if the set contains a specific item</summary>
        /// <param name="item">The item to check if the set contains.</param>
        /// <returns><see langword="true" /> if found;
        /// otherwise <see langword="false" />.</returns>
        bool Contains(T item);

		/// <summary>Determines whether the current set is a proper (strict)
		/// subset of a specified set.</summary>
		/// <param name="other">The set to compare to the current set.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns>
		///   <see langword="true" /> if the current set is a proper subset of
		///   other; otherwise <see langword="false" />.</returns>
		bool IsProperSubsetOf(IDenseOrderedSet<T> other);

		/// <summary>Determines whether the current set is a proper (strict)
		/// superset of a specified set.</summary>
		/// <param name="other">The set to compare to the current set.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns><see langword="true" /> if the set is a proper
		/// superset of other; otherwise <see langword="false" />.</returns>
		bool IsProperSupersetOf(IDenseOrderedSet<T> other);

		/// <summary>Determine whether the current set is a subset of a
        /// specified set.</summary>
		/// <param name="other">The set to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns><see langword="true" /> if the current set is a subset of
        /// other; otherwise <see langword="false" />.</returns>
		bool IsSubsetOf(IDenseOrderedSet<T> other);

		/// <summary>Determine whether the current set is a super set of a
        /// specified set.</summary>
		/// <param name="other">The set to compare to the current set</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns><see langword="true" /> if the current set is a subset of
        /// other; otherwise <see langword="false" />.</returns>
		bool IsSupersetOf(IDenseOrderedSet<T> other);

		/// <summary>Determines whether the current set overlaps with the
        /// specified set.</summary>
		/// <param name="other">The set to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns><see langword="true" />if the current set and other share
        /// at least one common element; otherwise, <see langword="false" />.
        /// </returns>
		bool Overlaps(IDenseOrderedSet<T> other);

		/// <summary>Determines whether the current set and the specified set
        /// contain the same elements.</summary>
		/// <param name="other">The set to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="other" /> is <see langword="null" />.</exception>
		/// <returns><see langword="true" /> if the current set is equal to
        /// other; otherwise, <see langword="false" />.</returns>
		bool SetEquals(IDenseOrderedSet<T> other);

		/// <summary>
        /// Returns the set which contains every element except those that are
        /// present in this set.
        /// </summary>
        /// <returns></returns>
		IDenseOrderedSet<T> Complement();

        /// <summary>
        /// Returns another set which contains elements of this set and the
        /// given set.
        /// </summary>
        /// <param name="other">Another set.</param>
        /// <returns>A set which contains elements of this set and the
        /// given set.</returns>
        IDenseOrderedSet<T> Union(IDenseOrderedSet<T> other);

		/// <summary>
		/// Returns another set which contains elements belonging to
		/// both this set and the given set.
		/// </summary>
		/// <param name="another">Another set.</param>
		/// <returns>A set which contains elements belonging to
		/// both this set and the given set.</returns>
		IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another);

		/// <summary>
		/// Returns a list of all disjoint intervals whose union makes up
		/// this set. The intervals are ordered from low to high in
		/// the sense of
		/// <see cref="DenseInterval{T}.IsLower(DenseInterval{T})"/> and
		/// <see cref="DenseInterval{T}.IsUpper(DenseInterval{T})"/>.
		/// </summary>
		/// <returns>An ordered immutable list of disjoint intervals whose union
		/// makes up this set. Intervals are ordered low to high.</returns>
		ImmutableList<DenseInterval<T>> AsDisjointIntervals();

        /// <summary>
        /// Gets the length of this set using the given measure.
        /// </summary>
        /// <param name="measureFunction">The measure. This function
        /// should be able to handle infinities as configured in 
        /// <see cref="TypeConfiguration"/>.</param>
        /// <returns>The length of this set.</returns>
        double GetLength(Func<T?, T?, double> measureFunction);

		/// <summary>
        /// Returns an ordered sequence of elements contained in this set
        /// spaced in accordance with the given sampler function.
        /// </summary>
        /// <param name="generator">The function which generates an element
        /// given another element. from another element by the distance wanted
        /// by the caller.</param>
        /// <returns>Elements sampled from this set. The first element
        /// is either the lowest element in this set if this set is closed
        /// or the smallest element in this set seperated from the lowest
        /// boundary of this set by the distance controlled by the generator.
        /// </returns>
		IEnumerable<T> Sample(Func<T, T> generator);
    }
}
