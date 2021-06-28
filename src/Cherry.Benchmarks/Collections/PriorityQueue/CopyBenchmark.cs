using BenchmarkDotNet.Attributes;
using Cherry.Collection;
using System.Collections.Generic;
using System.Linq;

namespace Cherry.Benchmarks.PriorityQueue
{
    public class CopyBenchmark : AbstractPriorityQueueBenchmark
    {
        private PriorityQueue<int> _referenceQueue;
        private List<int> _heapifiedList;

        public CopyBenchmark() : base(100000) { }

        [GlobalSetup]
        public void Setup()
        {
            _referenceQueue = new PriorityQueue<int>(_added);
            _heapifiedList = _referenceQueue.ToList();
        }

        [Benchmark(Baseline = true)]
        public PriorityQueue<int> CreateFromList()
        {
            return new(_heapifiedList);
        }

        [Benchmark]
        public PriorityQueue<int> CreateFromAnotherQueue()
        {
            return new(_referenceQueue);
        }
    }
}
