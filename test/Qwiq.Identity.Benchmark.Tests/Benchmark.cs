using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.Qwiq.Identity.Mapper;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Benchmark;
using Qwiq.Identity.Tests.Mocks;

namespace Microsoft.Qwiq.Identity.Benchmark.Tests
{
    [Config(typeof(BenchmarkConfig))]
    [TestClass]
    public class Benchmark
    {
        private IWorkItemMapperStrategy _strategy;
        private IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> _workItemMappings;

        [Setup]
        [TestInitialize]
        public void Setup()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            _strategy = new BulkIdentityAwareAttributeMapperStrategy(
                propertyInspector,
                new MockIdentityManagementService()
            );

            var generator = new WorkItemGenerator<MockWorkItem>(() => new MockWorkItem(), new[] { "Revisions", "Item", "AssignedTo" });
            generator.Generate();

            var assignees = new[]
                                {
                                    MockIdentityManagementService.Danj.DisplayName,
                                    MockIdentityManagementService.Adamb.DisplayName,
                                    MockIdentityManagementService.Chrisj.DisplayName,
                                    MockIdentityManagementService.Chrisjoh.DisplayName,
                                    MockIdentityManagementService.Chrisjohn.DisplayName,
                                    MockIdentityManagementService.Chrisjohns.DisplayName
                                };

            var sourceWorkItems = generator
                .Items
                // Run post-randomization to enable our scenario
                .Select(
                    s =>
                        {
                            var i = Randomizer.Instance.Next(0, assignees.Length - 1);
                            s[MockIdentityType.BackingField] = assignees[i];

                            return s;
                        });

            _workItemMappings = sourceWorkItems.Select(t => new KeyValuePair<IWorkItem, IIdentifiable>(t, new MockIdentityType())).ToList();
        }

        [Benchmark]
        public IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> Execute()
        {
            _strategy.Map(typeof(MockIdentityType), _workItemMappings, null);
            return _workItemMappings;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("Performance")]
        [TestCategory("Benchmark")]
        public void Execute_Identity_Mapping_Performance_Benchmark()
        {
            BenchmarkRunner.Run<Benchmark>();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("Performance")]
        public void Execute_Identity_Mapping()
        {
            Execute();
        }
    }
}
