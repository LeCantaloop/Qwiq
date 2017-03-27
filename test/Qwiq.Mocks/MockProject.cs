using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : IProject
    {


        public MockProject(IWorkItemStore store, INode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Store = store ?? throw new ArgumentNullException(nameof(store));

            WorkItemTypes = new[]
                                {
                                    new MockWorkItemType("Task"),
                                    new MockWorkItemType("Deliverable"),
                                    new MockWorkItemType("Scenario"),
                                    new MockWorkItemType("Customer Promise"),
                                    new MockWorkItemType("Bug"),
                                    new MockWorkItemType("Measure")
                                };

            AreaRootNodes = new[] { node };
            IterationRootNodes = new[] { CreateNodes(false) };
            Guid = Guid.NewGuid();
        }

        public MockProject(IWorkItemStore store)
            :this(store, CreateNodes(true))
        {
        }

        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(IWorkItemStore).")]
        public MockProject()
            : this(new MockWorkItemStore(), CreateNodes(true))
        {
        }

        public IEnumerable<INode> AreaRootNodes { get; set; }

        public Guid Guid { get; set; }

        public int Id { get; set; }

        public IEnumerable<INode> IterationRootNodes { get; set; }

        public string Name => "Mock";

        public Uri Uri { get; set; }

        public IEnumerable<IWorkItemType> WorkItemTypes { get; set; }

        public IWorkItemStore Store { get; }

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
