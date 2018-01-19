using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockProject : Project
    {
        internal const string ProjectName = "Mock Project";

        public MockProject(Guid id, string name, Uri uri, IWorkItemTypeCollection wits, IWorkItemClassificationNodeCollection<int> areas, IWorkItemClassificationNodeCollection<int> iterations)
            : base(
                   id,
                   name,
                   uri,
                   new Lazy<IWorkItemTypeCollection>(() => wits),
                   new Lazy<IWorkItemClassificationNodeCollection<int>>(() => areas),
                   new Lazy<IWorkItemClassificationNodeCollection<int>>(() => iterations))
        {
        }

        public MockProject(IWorkItemStore store)
            : base(
                Guid.NewGuid(),
                ProjectName,
                new Uri("http://localhost/projects/1"),
                new Lazy<IWorkItemTypeCollection>(() => new MockWorkItemTypeCollection(store)),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(() => WorkItemClassificationNodeCollectionBuilder.Build(NodeType.Area)),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(() => WorkItemClassificationNodeCollectionBuilder.Build(NodeType.Iteration))
            )
        {
        }
    }
}