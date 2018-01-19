using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq.Benchmark;
using Microsoft.Qwiq.Identity.Mocks;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using B = Microsoft.Qwiq.Identity.Benchmark.Tests.BENCHMARK_Given_a_set_of_WorkItems_with_a_BulkIdentityAwareAttributeMapperStrategy;

namespace Microsoft.Qwiq.Identity.Benchmark.Tests
{
    [TestClass]
    public class BENCHMARK_Given_a_set_of_WorkItems_with_a_BulkIdentityAwareAttributeMapperStrategy : BenchmarkContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            BenchmarkRunner.Run<Benchmark>();
        }

        [TestMethod]
#if DEBUG
        [Ignore]
#endif
        [TestCategory(Constants.TestCategory.Benchmark)]
        [TestCategory(Constants.TestCategory.Performance)]
        public void Execute_Identity_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
        public class Benchmark
        {
            private IWorkItemMapperStrategy _strategy;
            private Dictionary<IWorkItem, IIdentifiable<int?>> _workItemMappings;

            [GlobalSetup]
            public void SetupData()
            {
                var propertyInspector = new PropertyInspector(new PropertyReflector());
                _strategy = new BulkIdentityAwareAttributeMapperStrategy(
                                                                         propertyInspector,
                                                                         new MockIdentityManagementService()
                                                                        );

                var wis = new MockWorkItemStore();
                var generator = new WorkItemGenerator<MockWorkItem>(() => wis.Create(), new[] { "Revisions", "Item" });
                wis.Add(generator.Generate());

                _workItemMappings = generator.Items.ToDictionary(k => (IWorkItem) k, e => (IIdentifiable<int?>) new MockIdentityType());

            }

            [Benchmark]
            public IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> Execute()
            {
                _strategy.Map(typeof(MockIdentityType), _workItemMappings, null);
                return _workItemMappings;
            }
        }
    }
}

namespace Microsoft.Qwiq.Mapper.Tests
{
    [TestClass]
    public class Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy : ContextSpecification
    {
        private B.Benchmark _benchmark;

        public override void Given()
        {
            _benchmark = new B.Benchmark();
            _benchmark.SetupData();
        }

        public override void When()
        {
            _benchmark.Execute();
        }

        [TestMethod]
        public void Execute()
        {
            Assert.Inconclusive("There is no condition verified. This executes to ensure the benchmark code functions without exception.");
        }
    }
}
