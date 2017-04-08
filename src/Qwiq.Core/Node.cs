using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class Node : INode, IEquatable<INode>
    {
        private readonly Lazy<INodeCollection> _children;

        private readonly Lazy<INode> _parent;

        private readonly Lazy<string> _path;

        internal Node(
            int id,
            bool isAreaNode,
            bool isIterationNode,
            string name,
            Func<INode> parentFactory,
            Func<INode, IEnumerable<INode>> childrenFactory)
        {
            Id = id;
            IsAreaNode = isAreaNode;
            IsIterationNode = isIterationNode;
            Name = name;

            _parent = new Lazy<INode>(parentFactory);
            _children = new Lazy<INodeCollection>(() => new NodeCollection(childrenFactory(this)));
            _path = new Lazy<string>(() => ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\'));
        }

        internal Node(int id, bool isAreaNode, bool isIterationNode, string name)
            : this(id, isAreaNode, isIterationNode, name, () => null, n => Enumerable.Empty<INode>())
        {
        }

        public bool Equals(INode other)
        {
            return NodeComparer.Instance.Equals(this, other);
        }

        public virtual INodeCollection ChildNodes => _children.Value;

        public virtual bool HasChildNodes => ChildNodes.Any();

        public int Id { get; }

        public bool IsAreaNode { get; }

        public bool IsIterationNode { get; }

        public string Name { get; }

        public virtual INode ParentNode => _parent.Value;

        public virtual string Path => _path.Value;

        public virtual Uri Uri { get; }

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