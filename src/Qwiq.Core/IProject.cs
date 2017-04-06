using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IProject 
    {
        IEnumerable<INode> AreaRootNodes { get; }

        Guid Guid { get; }

        IEnumerable<INode> IterationRootNodes { get; }

        string Name { get; }

        Uri Uri { get; }

        IWorkItemTypeCollection WorkItemTypes { get; }
    }
}