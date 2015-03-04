using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public interface IProject
    {
        NodeCollection AreaRootNodes { get; }
        int Id { get; }
        NodeCollection IterationRootNodes { get; }
        string Name { get; }
        Uri Uri { get; }
        WorkItemTypeCollection WorkItemTypes { get; }
    }
}