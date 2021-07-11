using System;
using System.Collections.Generic;
using System.Linq;

namespace Cherry.Collections.Dense
{
    internal static class SetMath
    {
        public static List<DenseInterval<T>> Union<T>(
            IEnumerable<IDenseOrderedSet<T>> sets)
            where T : IComparable<T>
        {
            var orderedDisjointSets =
                sets.SelectMany(s => s.AsDisjointIntervals())
                .Where(s => !s.IsEmpty)
                .OrderBy(i => i.LowerEndpoint)
                .ThenBy(i => i.UpperEndpoint)
                .ToList();

            for (int i = 1; i < orderedDisjointSets.Count; i++)
            {
                var preceding = orderedDisjointSets[i - 1];
                var curr = orderedDisjointSets[i];
                if (curr.IsConnected(preceding))
                {
                    orderedDisjointSets[i - 1] = null!;
                    orderedDisjointSets[i] = new DenseInterval<T>(
                        preceding.LowerEndpoint, curr.UpperEndpoint);
                }
            }
            return orderedDisjointSets;
        }
    }
}
