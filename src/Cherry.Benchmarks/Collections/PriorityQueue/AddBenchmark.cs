using BenchmarkDotNet.Attributes;
using Cherry.Collections;
using System.Collections.Generic;

namespace Cherry.Benchmarks.PriorityQueue
{
    public class AddBenchmark : AbstractPriorityQueueBenchmark
    {
        private PriorityQueue<int> _priorityQueue;
        private SortedSet<int> _sortedSet;

        public AddBenchmark() : base(1000000) { }

        [IterationSetup]
        public void Setup()
        {
            _priorityQueue = new();
            _sortedSet = new();
        }

        [Benchmark(Baseline = true)]
        public int SortedSetAdd()
        {
            foreach (var item in _added)
            {
                _sortedSet.Add(item);
            }
            return _sortedSet.Count;
        }

        [Benchmark]
        public int PriorityQueueAdd()
        {
            foreach (var item in _added)
            {
                _priorityQueue.Enqueue(item);
            }
            return _priorityQueue.Count;
        }
    }
}
