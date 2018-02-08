using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;

namespace Qwiq.Benchmark
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.Clr.With(Jit.RyuJit).With(Platform.X64).With(new GcMode { Server = true }));
            Add(Job.Clr.With(Jit.RyuJit).With(Platform.X86).With(new GcMode { Server = true }));
            Add(Job.Clr.With(Jit.RyuJit).With(Platform.AnyCpu).With(new GcMode { Server = true }));

            // GC and Memory Allocation
            Add(new BenchmarkDotNet.Diagnosers.MemoryDiagnoser());
            Add(new InliningDiagnoser());

            // Checks whether any of the referenced assemblies is non-optimized
            Add(JitOptimizationsValidator.FailOnError);

            Add(StatisticColumn.AllStatistics);


        }
    }
}