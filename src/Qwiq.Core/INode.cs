using System;

namespace Microsoft.Qwiq
{
    public interface INode
    {
        INodeCollection ChildNodes { get; }

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