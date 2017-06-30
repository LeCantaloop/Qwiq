using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class NodeCollection : ReadOnlyObjectWithIdCollection<INode, int>, INodeCollection
    {
        internal NodeCollection([InstantHandle] [NotNull] IEnumerable<INode> nodes)
            :this(nodes.ToList())
        {
            Contract.Requires(nodes != null);
        }

        internal NodeCollection(List<INode> nodes)
            : base(nodes, node => node.Name)
        {
        }

        public bool Equals(INodeCollection other)
        {
            return Comparer.NodeCollection.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Comparer.NodeCollection.Equals(this, obj as INodeCollection);
        }

        public override int GetHashCode()
        {
            return Comparer.NodeCollection.GetHashCode(this);
        }
    }
}