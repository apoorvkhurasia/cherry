using System;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace CherryCollections.Benchmarks.PriorityQueue
{
    public class ContainsBenchmark : AbstractPriorityQueueBenchmark
    {
        private PriorityQueue<int> _priorityQueue;
        private SortedSet<int> _sortedSet;
        private List<int> _list;

        private List<int> _numsToCheck;

        public ContainsBenchmark() : base(5000)
        {
            var rnd = new Random();
            _numsToCheck = _added.Union(_notAdded)
                .OrderBy(c => rnd.Next())
                .Take(1000)
                .ToList();
        }

        [GlobalSetup]
        public void Setup()
        {
            _priorityQueue = new(_added);
            _sortedSet = new(_added);
            _list = new(_added);
        }

        [Benchmark(Baseline = true)]
        public int SortedSetContains()
        {
            return GetNumOfMatches(_sortedSet, _numsToCheck);
        }

        [Benchmark]
        public int PriorityQueueContains()
        {
            return GetNumOfMatches(_priorityQueue, _numsToCheck);
        }

        [Benchmark]
        public int ListContains()
        {
            return GetNumOfMatches(_list, _numsToCheck);
        }

        private static int GetNumOfMatches<T1>(
            ICollection<T1> @in, IEnumerable<T1> of)
        {
            return of.Where(@in.Contains).Count();
        }
    }
}
