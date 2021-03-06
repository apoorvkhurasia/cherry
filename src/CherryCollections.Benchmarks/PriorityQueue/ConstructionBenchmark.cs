using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

namespace CherryCollections.Benchmarks.PriorityQueue
{
    public class ConstructionBenchmark : AbstractPriorityQueueBenchmark
    {
        public ConstructionBenchmark() : base(100000) { }

        [Benchmark(Baseline = true)]
        public ICollection<int> SortedSetConstruction()
        {
            return new SortedSet<int>(_added);
        }

        [Benchmark]
        public ICollection<int> PriorityQueueConstruction()
        {
            return new PriorityQueue<int>(_notAdded);
        }
    }
}
