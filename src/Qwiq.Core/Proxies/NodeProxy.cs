using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class NodeProxy : INode
    {
        private NodeProxy()
        {
        }

        public IEnumerable<INode> ChildNodes { get; }

        public bool HasChildNodes { get; }

        public int Id { get; }

        public bool IsAreaNode { get; }

        public bool IsIterationNode { get; }

        public string Name { get; }

        public INode ParentNode { get; internal set; }

        public string Path { get; }

        public Uri Uri { get; }

        public override bool Equals(object obj)
        {
            return NodeComparer.Instance.Equals(this, obj as INode);
        }

        public override int GetHashCode()
        {
            return NodeComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}