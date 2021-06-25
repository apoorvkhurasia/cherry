using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

namespace CherryCollections.Benchmarks.PriorityQueue
{
    [MarkdownExporter]
    public abstract class AbstractPriorityQueueBenchmark
    {
        protected readonly List<int> _added = new();
        protected readonly List<int> _notAdded = new();

        protected AbstractPriorityQueueBenchmark(int smplSize)
        {
            var random = new Random();
            var uniques = new HashSet<int>();
            while (uniques.Count < 2 * smplSize)
            {
                uniques.Add(random.Next());
            }
            int i = 0;
            foreach (var number in uniques)
            {
                if (i++ < smplSize) _added.Add(number);
                else _notAdded.Add(number);
            }
        }
    }
}
