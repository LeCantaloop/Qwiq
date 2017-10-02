using System;

namespace Qwiq
{
    public interface IProject : IIdentifiable<Guid>, IResourceReference, INamed
    {
        IWorkItemClassificationNodeCollection<int> AreaRootNodes { get; }

        Guid Guid { get; }

        IWorkItemClassificationNodeCollection<int> IterationRootNodes { get; }

        IWorkItemTypeCollection WorkItemTypes { get; }

        IQueryFolderCollection QueryHierarchy { get; }
    }
}