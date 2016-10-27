using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface INode
    {
        IEnumerable<INode> ChildNodes { get; }
        bool HasChildNodes { get; }
        int Id { get; }
        bool IsAreaNode { get; }
        bool IsIterationNode { get; }
        string Name { get; }
        INode ParentNode { get; }
        string Path { get; }
        Uri Uri { get; }
    }
}
