using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : Project
    {
        private static readonly string ProjectName = "Mock Project";

        public MockProject()
            : base(
                Guid.NewGuid(),
                ProjectName,
                new Uri("http://localhost"),
                new Lazy<IWorkItemTypeCollection>(
                    () => new WorkItemTypeCollection(
                                                     new MockWorkItemType(WorkItemTypeDefinitions.Task),
                                                     new MockWorkItemType(WorkItemTypeDefinitions.Deliverable),
                                                     new MockWorkItemType(WorkItemTypeDefinitions.Scenario),
                                                     new MockWorkItemType(WorkItemTypeDefinitions.CustomerPromise),
                                                     new MockWorkItemType(WorkItemTypeDefinitions.Bug),
                                                     new MockWorkItemType(WorkItemTypeDefinitions.Measure))),
                new Lazy<INodeCollection>(() => CreateNodes(true) ),
                new Lazy<INodeCollection>(() => CreateNodes(false) ))
        {
        }

        public MockProject(IEnumerable<IWorkItemType> workItemTypes)
            : base(
                Guid.NewGuid(),
                ProjectName,
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
                n => new[] { new Node(3, area, !area, "L2", () => n, c => Enumerable.Empty<INode>()) });


            return new NodeCollection( root);
        }
    }
}