using System;
using System.Collections.Generic;
using System.Linq;

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
                new Lazy<INodeCollection>(() => CreateNodes(true) ),
                new Lazy<INodeCollection>(() => CreateNodes(false) ))
        {
        }

        public MockProject(IEnumerable<IWorkItemType> workItemTypes)
            : base(
                Guid.NewGuid(),
                "Mock Project",
                new Uri("http://localhost"),
                new Lazy<IWorkItemTypeCollection>(() => new WorkItemTypeCollection(workItemTypes)),
                new Lazy<INodeCollection>(() => CreateNodes(true) ),
                new Lazy<INodeCollection>(() => CreateNodes(false) ))
        {
        }

        public MockProject(IWorkItemType workItemType)
            : this(new[] { workItemType })
        {
        }

        private static INodeCollection CreateNodes(bool area)
        {
            var root = new Node(1, area, !area, "Root");
            var l1 = new Node(2, area, !area, "L1",
                () => root,
                n => new[] { new Node(3, area, !area, "L2", () => n, (c) => Enumerable.Empty<INode>()) });


            return new NodeCollection( root);
        }
    }
}