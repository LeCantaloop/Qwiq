using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq.Benchmark;
using Microsoft.Qwiq.Identity.Mapper;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Identity.Tests.Mocks;

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
        [TestCategory(Constants.TestCategory.Benchmark)]
        [TestCategory(Constants.TestCategory.Performance)]
        [TestCategory("localOnly")]
        public void Execute_Identity_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
        public class Benchmark
        {
            private IWorkItemMapperStrategy _strategy;
            private IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> _workItemMappings;

            [Setup]
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

                _workItemMappings = generator.Items.Select(t => new KeyValuePair<IWorkItem, IIdentifiable<int?>>(t, new MockIdentityType())).ToList();

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
