using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class NodeCollection : ReadOnlyCollectionWithId<INode, int>, INodeCollection
    {
        internal NodeCollection(params INode[] nodes)
            : this(nodes as IEnumerable<INode>)
        {
        }

        internal NodeCollection(IEnumerable<INode> nodes)
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