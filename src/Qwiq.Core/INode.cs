using System;

namespace Microsoft.Qwiq
{
    public interface INode : IIdentifiable<int>, IResourceReference
    {
        INodeCollection ChildNodes { get; }

        bool HasChildNodes { get; }

        string Name { get; }

        INode ParentNode { get; }

        string Path { get; }

        NodeType Type { get; }
    }
}