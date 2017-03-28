using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Proxies;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : Project
    {
        public MockProject(IWorkItemStore store)
            : base(
                BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0),
                Guid.NewGuid(),
                "Mock",
                new Uri("http://localhost"),
                store,
                new Lazy<IEnumerable<IWorkItemType>>(
                    () => new[]
                              {
                                  new MockWorkItemType("Task"),
                                  new MockWorkItemType("Deliverable"),
                                  new MockWorkItemType("Scenario"),
                                  new MockWorkItemType("Customer Promise"),
                                  new MockWorkItemType("Bug"),
                                  new MockWorkItemType("Measure")
                              }),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(true) }),
                new Lazy<IEnumerable<INode>>(() => new[] { CreateNodes(false) }))
        {
        }

        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(IWorkItemStore).")]
        public MockProject()
            : this(new MockWorkItemStore())
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