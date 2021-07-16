using System;

namespace Cherry.Collections.Dense
{
    public static class StandardMeasures
    {
        /// <summary>
        /// The standard borel measure defines the length 
        /// of the half open interval (a, b] as b - a.
        /// </summary>
        public static Func<double, double, double> Borel { get; }
            = (start, end) => end >= start ? end - start : 0d;
    }
}
