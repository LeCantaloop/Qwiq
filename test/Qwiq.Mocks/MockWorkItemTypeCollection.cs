using System.Collections.Generic;


namespace Qwiq.Mocks
{
    internal class MockWorkItemTypeCollection : WorkItemTypeCollection
    {
        public MockWorkItemTypeCollection(IWorkItemStore store)
            : base((List<IWorkItemType>)null!)
        {
            ItemFactory = () => new[]
                                    {
                                        new MockWorkItemType(WorkItemTypeDefinitions.Task, null, store),
                                        new MockWorkItemType(WorkItemTypeDefinitions.Deliverable, null, store),
                                        new MockWorkItemType(WorkItemTypeDefinitions.Scenario, null, store),
                                        new MockWorkItemType(WorkItemTypeDefinitions.CustomerPromise, null, store),
                                        new MockWorkItemType(WorkItemTypeDefinitions.Bug, null, store),
                                        new MockWorkItemType(WorkItemTypeDefinitions.Measure, null, store)
                                    };
        }
    }
}