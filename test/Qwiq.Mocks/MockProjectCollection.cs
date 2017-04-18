using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    internal class MockProjectCollection : ProjectCollection
    {
        public MockProjectCollection(IWorkItemStore store)
            : base(new MockProject(store, new Node(1, false, false, MockProject.ProjectName, new Uri("http://localhost/projects/1"))))
        {
        }

        public MockProjectCollection(IEnumerable<IProject> projects)
            : base(projects)
        {
        }
    }

    internal class MockWorkItemTypeCollection : WorkItemTypeCollection
    {
        public MockWorkItemTypeCollection(IWorkItemStore store)
            : base((IWorkItemType[])null)
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