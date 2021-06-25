using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace CherryCollections.Benchmarks.PriorityQueue
{
    public class DequeueBenchmark : AbstractPriorityQueueBenchmark
    {
        private PriorityQueue<int> _priorityQueue;
        private SortedSet<int> _sortedSet;

        public DequeueBenchmark() : base(10000) { }

        [IterationSetup]
        public void Setup()
        {
            _priorityQueue = new(_added);
            _sortedSet = new(_added);
        }

        [Benchmark(Baseline = true)]
        public int SortedSetRemoveHead()
        {
            while (_sortedSet.Count > 0)
            {
                _sortedSet.Remove(_sortedSet.Min());
            }
            return _sortedSet.Count;
        }

        [Benchmark]
        public int PriorityQueueDequeue()
        {
            while (_priorityQueue.Count > 0)
            {
                _priorityQueue.Dequeue();
            }
            return _priorityQueue.Count;
        }
    }
}
