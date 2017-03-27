using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IProject
    {
        IEnumerable<INode> AreaRootNodes { get; }
        int Id { get; }
        IEnumerable<INode> IterationRootNodes { get; }
        string Name { get; }
        Uri Uri { get; }
        IEnumerable<IWorkItemType> WorkItemTypes { get; }
        IWorkItemStore Store { get; }
    }
}
