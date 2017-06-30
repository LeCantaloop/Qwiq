using System;

namespace Microsoft.Qwiq
{
    public interface INode : IIdentifiable<int>
    {
        INodeCollection ChildNodes { get; }

        bool HasChildNodes { get; }

        bool IsAreaNode { get; }

        bool IsIterationNode { get; }

        string Name { get; }

        INode ParentNode { get; }

        string Path { get; }

        Uri Uri { get; }
    }
}