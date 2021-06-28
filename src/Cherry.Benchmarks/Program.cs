using Cherry.Benchmarks.PriorityQueue;
#if DEBUG
using BenchmarkDotNet.Configs;
#endif
using BenchmarkDotNet.Running;

namespace Cherry.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                .Run(args, new DebugInProcessConfig());
#endif
            BenchmarkRunner.Run<ConstructionBenchmark>();
            BenchmarkRunner.Run<AddBenchmark>();
            BenchmarkRunner.Run<ContainsBenchmark>();
            BenchmarkRunner.Run<DequeueBenchmark>();
            BenchmarkRunner.Run<CopyBenchmark>();
        }
    }
}
