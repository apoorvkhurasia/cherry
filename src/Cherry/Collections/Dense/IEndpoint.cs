using System;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Represents the endpoint of an <see cref="Interval{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEndpoint<T> where T : IComparable<T>
    {
        /// <summary>
        /// Returns true if and only if the given value is precisely
        /// on this endpoint as determined by the
        /// <see cref="IComparable{T}.CompareTo(T?)"/> method.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if and only if the given value is equal to this
        /// endpoint as determined by the comparision ordering.</returns>
        bool IsValueOnTheEndPoint(T value);

    }
}
