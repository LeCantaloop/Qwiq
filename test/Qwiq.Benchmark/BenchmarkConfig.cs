using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;

namespace Qwiq.Benchmark
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
#if NETFRAMEWORK
            AddJob(Job.Clr.WithJit(Jit.RyuJit).WithPlatform(Platform.X64).WithGcServer(true));
            AddJob(Job.Clr.WithJit(Jit.RyuJit).WithPlatform(Platform.X86).WithGcServer(true));
            AddJob(Job.Clr.WithJit(Jit.RyuJit).WithPlatform(Platform.AnyCpu).WithGcServer(true));
#else
            AddJob(Job.Default.WithJit(Jit.RyuJit).WithPlatform(Platform.X64).WithGcServer(true));
            AddJob(Job.Default.WithJit(Jit.RyuJit).WithPlatform(Platform.AnyCpu).WithGcServer(true));
#endif

            // GC and Memory Allocation
            AddDiagnoser(MemoryDiagnoser.Default);

            // Checks whether any of the referenced assemblies is non-optimized
            AddValidator(JitOptimizationsValidator.FailOnError);

            AddColumn(StatisticColumn.AllStatistics);
        }
    }
}