using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Qwiq.Benchmark
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.Clr.With(Jit.RyuJit).With(Platform.X64).With(new GcMode { Server = true }));
            Add(Job.Clr.With(Jit.LegacyJit).With(Platform.X64).With(new GcMode { Server = true }));

            Add(Job.Clr.With(Jit.RyuJit).With(Platform.X86).With(new GcMode { Server = true }));
            Add(Job.Clr.With(Jit.LegacyJit).With(Platform.X86).With(new GcMode { Server = true }));

            Add(new BenchmarkDotNet.Diagnosers.MemoryDiagnoser());
            Add(new InliningDiagnoser());

            Add(StatisticColumn.AllStatistics);

        }
    }
}