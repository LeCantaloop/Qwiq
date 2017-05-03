using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Uri uri,
            Func<INode> parentFactory,
            Func<INode, IEnumerable<INode>> childrenFactory)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (parentFactory == null) throw new ArgumentNullException(nameof(parentFactory));
            if (childrenFactory == null) throw new ArgumentNullException(nameof(childrenFactory));

            Id = id;
            IsAreaNode = isAreaNode;
            IsIterationNode = isIterationNode;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Uri = uri;

            _parent = new Lazy<INode>(parentFactory);
            _children = new Lazy<INodeCollection>(() => new NodeCollection(childrenFactory(this)));
            _path = new Lazy<string>(() => ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\'));
        }

        internal Node(int id, bool isAreaNode, bool isIterationNode, string name, Uri uri)
            : this(id, isAreaNode, isIterationNode, name, uri, () => null, n => Enumerable.Empty<INode>())
        {
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

        [DebuggerStepThrough]
        public bool Equals(INode other)
        {
            return NodeComparer.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return NodeComparer.Default.Equals(this, obj as INode);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return NodeComparer.Default.GetHashCode(this);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return Path;
        }
    }
}