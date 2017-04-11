using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class NodeCollection : ReadOnlyListWithId<INode, int>, INodeCollection
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
            return Comparer.NodeCollectionComparer.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Comparer.NodeCollectionComparer.Equals(this, obj as INodeCollection);
        }

        public override int GetHashCode()
        {
            return Comparer.NodeCollectionComparer.GetHashCode(this);
        }
    }
}