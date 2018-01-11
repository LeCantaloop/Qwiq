using System;

namespace Microsoft.Qwiq
{
    public interface IProject : IIdentifiable<Guid>, IResourceReference
    {
        INodeCollection AreaRootNodes { get; }

        Guid Guid { get; }

        INodeCollection IterationRootNodes { get; }

        string Name { get; }

        IWorkItemTypeCollection WorkItemTypes { get; }
    }
}