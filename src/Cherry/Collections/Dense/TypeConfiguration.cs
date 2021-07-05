using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cherry.Collections.Dense
{
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

        public static void RegisterPositiveInfinityInstance<T>(T instance)
        {
            _positiveInfinities[typeof(T)] = instance!;
        }

        public static void RegisterNegativeInfinityInstance<T>(T instance)
        {
            _negativeInfinities[typeof(T)] = instance!;
        }

        public static bool IsPositiveInfinity<T>(T instance) =>
            TryGetPositiveInfinity<T>(out var t)
            && Comparer<T>.Default.Compare(instance, t) == 0;

        public static bool IsNegativeInfinity<T>(T instance) =>
            TryGetNegativeInfinity<T>(out var t)
            && Comparer<T>.Default.Compare(instance, t) == 0;

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
