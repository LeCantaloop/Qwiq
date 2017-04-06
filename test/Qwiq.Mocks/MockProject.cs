using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : Project
    {
        public MockProject()
            : base(
                Guid.NewGuid(),
                "Mock Project",
                new Uri("http://localhost"),
                new Lazy<IWorkItemTypeCollection>(
                    () => new WorkItemTypeCollection(
                                                     new MockWorkItemType("Task"),
                                                     new MockWorkItemType("Deliverable"),
                                                     new MockWorkItemType("Scenario"),
                                                     new MockWorkItemType("Customer Promise"),
                                                     new MockWorkItemType("Bug"),
                                                     new MockWorkItemType("Measure"))),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(true) }),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(false) }))
        {
        }

        public MockProject(IEnumerable<IWorkItemType> workItemTypes)
            : base(
                Guid.NewGuid(),
                "Mock Project",
                new Uri("http://localhost"),
                new Lazy<IWorkItemTypeCollection>(() => new WorkItemTypeCollection(workItemTypes)),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(true) }),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(false) }))
        {
        }

        public MockProject(IWorkItemType workItemType)
            : this(new[] { workItemType })
        {
        }

        private static MockNode CreateNodes(bool area)
        {
            var root = new MockNode("Root", true, !area);
            var l1 = new MockNode("L1", true, !area) { ParentNode = root };
            var l2 = new MockNode("L2", true, !area) { ParentNode = l1 };

            root.ChildNodes = new[] { l1 };
            l1.ChildNodes = new[] { l2 };
            return root;
        }
    }
}