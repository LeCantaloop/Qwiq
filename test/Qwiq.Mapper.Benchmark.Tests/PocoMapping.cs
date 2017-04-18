using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq.Benchmark;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using B = Microsoft.Qwiq.Mapper.Benchmark.Tests.BENCHMARK_Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy;

namespace Microsoft.Qwiq.Mapper.Benchmark.Tests
{
    [TestClass]
    public class BENCHMARK_Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy : BenchmarkContextSpecification
    {
        public override void When()
        {
            BenchmarkRunner.Run<Benchmark>();
        }


        [TestMethod]
        [TestCategory(Constants.TestCategory.Benchmark)]
        [TestCategory(Constants.TestCategory.Performance)]
        [TestCategory("localOnly")]
        public void Execute_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
        public class Benchmark
        {
            private WorkItemMapper _mapper;
            private IEnumerable<IWorkItem> _items;

            [Setup]
            public void SetupData()
            {
                var propertyInspector = new PropertyInspector(new PropertyReflector());
                var typeParser = TypeParser.Default;
                var mappingStrategies = new IWorkItemMapperStrategy[]
                                            { new AttributeMapperStrategy(propertyInspector, typeParser) };
                _mapper = new WorkItemMapper(mappingStrategies);

                var wis = new MockWorkItemStore();
                var generator = new WorkItemGenerator<MockWorkItem>(() => wis.Create(), new[] { "Revisions", "Item" });
                _items = generator.Generate();
                wis.Add(_items);
            }

            [Benchmark]
            public IList Execute()
            {
                return _mapper.Create<MockModel>(_items).ToList();
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
