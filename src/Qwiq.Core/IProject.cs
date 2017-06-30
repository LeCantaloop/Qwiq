using System;

namespace Microsoft.Qwiq
{
    public interface IProject : IIdentifiable<Guid>
    {
        INodeCollection AreaRootNodes { get; }

        Guid Guid { get; }

        INodeCollection IterationRootNodes { get; }

        string Name { get; }

        Uri Uri { get; }

        IWorkItemTypeCollection WorkItemTypes { get; }
    }
}