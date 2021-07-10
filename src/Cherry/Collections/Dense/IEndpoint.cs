using System;

namespace Cherry.Collections.Dense
{
    /// <summary>
    /// Represents a boundary point for a <see cref="DenseInterval{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEndpoint<T> : IComparable<T> where T : IComparable<T>
    {
        /// <summary>
        /// The value of the endpoint. Can be null for an infinite 
        /// endpoint if the type <typeparamref name="T"/> treats null
        /// as negative infinity. See <seealso cref="TypeConfiguration"/>.
        /// </summary>
        T? Value { get; }

        /// <summary>
        /// Returns <see langword="true"/> if and only if this endpoint
        /// includes the value at which it is located. <see langword="false">
        /// otherwise.</see>
        /// </summary>
        bool IsInclusive { get; }

        /// <summary>
        /// Returns <see langword="true"/> if and only if this endpoint
        /// is located at negative infinity. <see langword="false">
        /// otherwise.</see>
        /// </summary>
        bool IsInfinite { get; }
    }
}