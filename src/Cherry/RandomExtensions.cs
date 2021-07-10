using System;

namespace Cherry
{
    /// <summary>
    /// Some extension methods on <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns -1 or 1 with equal probability.
        /// </summary>
        /// <param name="random"></param>
        /// <returns>-1 or 1 with equal probability.</returns>
        public static int NextSign(this Random random)
        {
            return random.NextDouble() < 0.5 ? -1 : 1;
        }
    }
}
