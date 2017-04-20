using System;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq.Benchmark;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mapper.Benchmark.Tests;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

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
        public void Execute_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
        public class Benchmark
        {
            private WorkItemMapper _mapper;
            private IEnumerable<IWorkItem> _items;

            private IEnumerable<IWorkItem> _item;

            private Type _type;

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
                _items = generator.Generate(1);
                wis.Add(_items);

                _item = new[] { _items.First() };
                _type = typeof(MockModel);
            }

            [Benchmark(Baseline = true)]
            public IEnumerable<MockModel> Generic()
            {
                return _mapper.Create<MockModel>(_item).ToList();
            }

            [Benchmark]
            public IEnumerable<IIdentifiable<int?>> NonGeneric()
            {
                return _mapper.Create(_type, _item).ToList();
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

        private IEnumerable<MockModel> _genericResult;

        private IEnumerable<IIdentifiable<int?>> _nonGenericResult;

        public override void Given()
        {
            _benchmark = new B.Benchmark();
            _benchmark.SetupData();
        }

        public override void When()
        {
            _genericResult = _benchmark.Generic();
            _nonGenericResult = _benchmark.NonGeneric();
        }

        [TestMethod]
        public void Execute()
        {
            _genericResult.ShouldContainOnly(_nonGenericResult);
        }
    }
}
