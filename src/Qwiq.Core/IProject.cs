using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IProject
    {
        IEnumerable<INode> AreaRootNodes { get; }

        Guid Guid { get; }

        int Id { get; }

        IEnumerable<INode> IterationRootNodes { get; }

        string Name { get; }

        IWorkItemStore Store { get; }

        Uri Uri { get; }

        IEnumerable<IWorkItemType> WorkItemTypes { get; }
    }
}