using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Proxies
{
    public class NodeProxy : INode, IComparer<INode>, IEquatable<INode>
    {
        public IEnumerable<INode> ChildNodes { get; internal set; }

        public bool HasChildNodes { get; internal set; }

        public int Id { get; internal set; }

        public bool IsAreaNode { get; internal set; }

        public bool IsIterationNode { get; internal set; }

        public string Name { get; internal set; }

        public INode ParentNode { get; internal set; }

        public string Path { get; internal set; }

        public Uri Uri { get; internal set; }

        public override bool Equals(object obj)
        {
            return NodeComparer.Instance.Equals(this, obj as INode);
        }

        public override int GetHashCode()
        {
            return NodeComparer.Instance.GetHashCode(this);
        }

        public int Compare(INode x, INode y)
        {
            return NodeComparer.Instance.Compare(x, y);
        }

        public bool Equals(INode other)
        {
            return NodeComparer.Instance.Equals(this, other);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}