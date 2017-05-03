using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    internal class MockProjectCollection : ProjectCollection
    {
        public MockProjectCollection(IWorkItemStore store)
            : this(new MockProject(store))
        {
        }

        public MockProjectCollection(MockProject project)
            : this(new[] { (IProject)project }.ToList())
        {
        }

        public MockProjectCollection(List<IProject> projects)
            : base(projects)
        {
        }
    }

    internal class MockWorkItemTypeCollection : WorkItemTypeCollection
    {
        public MockWorkItemTypeCollection(IWorkItemStore store)
            : base((List<IWorkItemType>)null)
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