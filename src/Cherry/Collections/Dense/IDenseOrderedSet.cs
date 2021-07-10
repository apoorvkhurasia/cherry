using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace  Cherry.Collections.Dense
{
	/// <summary>
	/// Represents a set that holds uncountably many items. The items in the
    /// set must have a notion of ordering and must be dense in the mathematical
    /// sense. The latter requirement means that between any two items there
    /// be an uncountably infinite number of more items. Integers do not qualify
    /// for this but real numbers do. There are two ways in items can be
    /// ordered:
	/// <list type="bullet">
	///		<item>They can implement <see cref="IComparable{T}"/></item>
    ///		<item>An <see cref="IComparer{T}"/> or <see cref="Comparison{T}"/>
    ///		can be provided by the caller. Care must be taken to use the
    ///		same comparision method consistently in all places where these
    ///		items can be ordered. Implementations provided by Cherry will
    ///		fail fast where possible when different comparision methods will
    ///		be mixed for the same type of items. However this is not a hard
    ///		guarentee as these implementations will interact with objects
    ///		not defined in this library.
    ///		</item>
    ///	</list>
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
		/// <param name="another">Another set.</param>
		/// <returns>A set which contains elements of this set and the
		/// given set.</returns>
		IDenseOrderedSet<T> Union(IDenseOrderedSet<T> another);

		// <summary>
		/// Returns another set which contains elements belonging to
		/// both this set and the given set.
		/// </summary>
		/// <param name="another">Another set.</param>
		/// <returns>A set which contains elements belonging to
		/// both this set and the given set.</returns>
		IDenseOrderedSet<T> Intersect(IDenseOrderedSet<T> another);

		/// <summary>
        /// Gets a set of disjoint intervals whose union makes up
        /// this set. This set is not unique and any such set can be returned
        /// by implementations. There is no guarentee that the same set will
        /// be returned by the same instance on repeated calls.
        /// </summary>
        /// <returns></returns>
		ImmutableList<DenseInterval<T>> AsDisjointIntervals();
	}
}
