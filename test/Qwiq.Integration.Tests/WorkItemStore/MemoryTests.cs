using System;

using JetBrains.dotMemoryUnit;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore
{
    [TestClass]
    public class Given_a_SOAP_and_REST_WorkItemStore : WorkItemStoreComparisonContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            Soap.Dispose();
            Rest.Dispose();

            GC.Collect();
        }

        [TestMethod]
        [DotMemoryUnit(CollectAllocations = true, FailIfRunWithoutSupport = false)]
        public void The_instances_are_removed_from_memory_after_Dispose()
        {
            dotMemory.Check(
                            memory =>
                                {
                                    memory.GetObjects(p => p.Type.Is(typeof(Client.Soap.WorkItemStore), typeof(Client.Rest.WorkItemStore)))
                                          .ObjectsCount.ShouldEqual(0);
                                });
        }
    }
}
