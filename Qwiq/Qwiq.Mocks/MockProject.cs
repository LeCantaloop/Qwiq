using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockProject : IProject
    {
        public MockProject()
        {
            WorkItemTypes = new[]
                                {
                                    new MockWorkItemType("Task"),
                                    new MockWorkItemType("Deliverable"),
                                    new MockWorkItemType("Scenario"),
                                    new MockWorkItemType("Customer Promise"),
                                    new MockWorkItemType("Bug"),
                                    new MockWorkItemType("Measure")
                                };

            AreaRootNodes = new[] { CreateNodes(true) };
            IterationRootNodes = new[] { CreateNodes(false) };
        }

        public IEnumerable<INode> AreaRootNodes { get; set; }

        public int Id { get; set; }

        public IEnumerable<INode> IterationRootNodes { get; set; }

        public string Name => "Mock";

        public Uri Uri { get; set; }

        public IEnumerable<IWorkItemType> WorkItemTypes { get; set; }

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