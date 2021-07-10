using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cherry
{
    /// <summary>
    /// This class allows you to define custom positive and negative
    /// infinity values for any type. All Cherry classes libraries will use this
    /// configuration to check for infinities. No attempt will be made to 
    /// influence the behaviour of other classes.
    /// </summary>
    public static class TypeConfiguration
    {
        private static readonly ConcurrentDictionary<Type, object>
            _positiveInfinities = new();
        private static readonly ConcurrentDictionary<Type, object>
            _negativeInfinities = new();

        static TypeConfiguration()
        {
            _positiveInfinities[typeof(double)] = double.PositiveInfinity;
            _negativeInfinities[typeof(double)] = double.NegativeInfinity;
        }

        /// <summary>
        /// Register a given instance as a positive infinity for its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance to resgier.</param>
        public static void RegisterPositiveInfinityInstance<T>(T instance)
        {
            _positiveInfinities[typeof(T)] = instance!;
        }

        /// <summary>
        /// Register a given instance as a negative infinity for its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance to resgier.</param>
        public static void RegisterNegativeInfinityInstance<T>(T instance)
        {
            _negativeInfinities[typeof(T)] = instance!;
        }

        /// <summary>
        /// Returns <see langword="true" /> if and only if the given instance
        /// has been registered as a positive infinity for its type.
        /// <see langword="false" /> otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns><see langword="true" /> if and only if the given instance
        /// has been registered as a positive infinity for its type.
        /// <see langword="false" /> otherwise.</returns>
        public static bool IsPositiveInfinity<T>(T instance) =>
            TryGetPositiveInfinity<T>(out var t)
            && Comparer<T>.Default.Compare(instance, t) == 0;

        /// <summary>
        /// Returns <see langword="true" /> if and only if the given instance
        /// has been registered as a negative infinity for its type.
        /// <see langword="false" /> otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns><see langword="true" /> if and only if the given instance
        /// has been registered as a negative infinity for its type.
        /// <see langword="false" /> otherwise.</returns>
        public static bool IsNegativeInfinity<T>(T instance) =>
            TryGetNegativeInfinity<T>(out var t)
            && Comparer<T>.Default.Compare(instance, t) == 0;

        /// <summary>
        /// Tries to get the registered positive infinity value if one has
        /// been registered for its type (for example, by using 
        /// <see cref="RegisterPositiveInfinityInstance{T}(T)"/>).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infinity">The parameter which should be assigned
        /// the positive infinity if one has been registered for its
        /// type.</param>
        /// <returns><see langword="true" /> if and only if an instance
        /// was registered as a positive infinity for its type.
        /// <see langword="false"/> otherwise.</returns>
        public static bool TryGetPositiveInfinity<T>(out T infinity)
        {
            if (_positiveInfinities.TryGetValue(typeof(T), out var t))
            {
                infinity = (T)t;
                return true;
            }
            else
            {
                infinity = default!;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the registered negative infinity value if one has
        /// been registered for its type (for example, by using 
        /// <see cref="RegisterNegativeInfinityInstance{T}(T)"/>).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infinity">The parameter which should be assigned
        /// the negative infinity if one has been registered for its
        /// type.</param>
        /// <returns><see langword="true" /> if and only if an instance
        /// was registered as a negative infinity for its type.
        /// <see langword="false"/> otherwise.</returns>
        public static bool TryGetNegativeInfinity<T>(out T infinity)
        {
            if (_negativeInfinities.TryGetValue(typeof(T), out var t))
            {
                infinity = (T)t;
                return true;
            }
            else
            {
                infinity = default!;
                return false;
            }
        }
    }
}
