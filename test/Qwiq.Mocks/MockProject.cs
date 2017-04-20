using System;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : Project
    {
        internal const string ProjectName = "Mock Project";

        public MockProject(Guid id, string name, Uri uri, IWorkItemTypeCollection wits, INodeCollection areas, INodeCollection iterations)
            : base(
                   id,
                   name,
                   uri,
                   new Lazy<IWorkItemTypeCollection>(() => wits),
                   new Lazy<INodeCollection>(() => areas),
                   new Lazy<INodeCollection>(() => iterations))
        {
        }

        internal MockProject(IWorkItemStore store, INode node)
            : base(
                   Guid.NewGuid(),
                   node.Name,
                   node.Uri,
                   new Lazy<IWorkItemTypeCollection>(() => new MockWorkItemTypeCollection(store)),
                   new Lazy<INodeCollection>(() => CreateNodes(true)),
                   new Lazy<INodeCollection>(() => CreateNodes(false)))
        {
        }

        private static INodeCollection CreateNodes(bool area)
        {
            var root = new Node(1, area, !area, "Root", new Uri("http://localhost/nodes/1"));
            new Node(
                     2,
                     area,
                     !area,
                     "L1",
                     new Uri("http://localhost/nodes/2"),
                     () => root,
                     n => new[]
                              {
                                  new Node(
                                           3,
                                           area,
                                           !area,
                                           "L2",
                                           new Uri("http://localhost/nodes/3"),
                                           () => n,
                                           c => Enumerable.Empty<INode>())
                              });

            return new NodeCollection(root);
        }
    }
}