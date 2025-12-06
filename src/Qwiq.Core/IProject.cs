using System;
using System.Diagnostics.CodeAnalysis;

namespace Qwiq
{
    /// <summary>
    /// Represents a Team Project in TFS/Azure DevOps.
    /// </summary>
    public interface IProject : IIdentifiable<Guid>, IResourceReference, INamed
    {
        IWorkItemClassificationNodeCollection<int> AreaRootNodes { get; }

        /// <summary>
        /// Gets the unique identifier for this project.
        /// </summary>
        [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Guid property name matches TFS/Azure DevOps API naming convention.")]
        Guid Guid { get; }

        IWorkItemClassificationNodeCollection<int> IterationRootNodes { get; }

        IWorkItemTypeCollection WorkItemTypes { get; }

        IQueryFolderCollection QueryHierarchy { get; }
    }
}