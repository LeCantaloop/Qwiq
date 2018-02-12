using System.Linq;

using Qwiq.Mapper.Benchmark.Tests;
using Qwiq.Mapper.Mocks;
using Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Mapper.AttributeMapper
{
    [TestClass]
    public class Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy : ContextSpecification
    {
        private BENCHMARK_Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy.Benchmark _benchmark;

        private MockModel _genericResult;

        private IIdentifiable<int?> _nonGenericResult;

        public override void Given()
        {
            _benchmark = new BENCHMARK_Given_a_set_of_WorkItems_with_an_AttributeMapperStrategy.Benchmark();
            _benchmark.SetupData();
        }

        public override void When()
        {
            _genericResult = _benchmark.Generic().First();
            _nonGenericResult = _benchmark.NonGeneric().First();
        }

        [TestMethod]
        public void Execute()
        {
            var ng = (MockModel)_nonGenericResult;
            var g = _genericResult;

            ng.WorkItemType.ShouldEqual(g.WorkItemType);
            ng.Milestone.ShouldEqual(g.Milestone);
        }
    }
}