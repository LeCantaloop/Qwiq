using System.Diagnostics;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Benchmark
{
    public abstract class BenchmarkContextSpecification : ContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            if (Debugger.IsAttached)
                Assert.Fail("Never should use an attached debugger (e.g. Visual Studio or WinDbg) during the benchmarking.");

#if DEBUG
            Assert.Fail(
                        "Never use the Debug build for benchmarking. Never. The debug version of the target method can run 10–100 times slower.");
#endif
        }
    }
}