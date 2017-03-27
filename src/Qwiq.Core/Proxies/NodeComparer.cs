using System;
using System.Linq;

namespace Microsoft.Qwiq.Proxies
{
    public class NodeComparer : GenericComparer<INode>
    {
        public static readonly NodeComparer Instance = Nested.Instance;

        private NodeComparer()
        {
        }

        public override bool Equals(INode x, INode y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.HasChildNodes == y.HasChildNodes
                   && x.Id == y.Id
                   && x.IsAreaNode == y.IsAreaNode
                   && x.IsIterationNode == y.IsIterationNode
                   && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && Instance.Equals(x.ParentNode, y.ParentNode)
                   && string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase)
                   && x.ChildNodes.All(p=>y.ChildNodes.Contains(p, Instance));
        }

        public override int GetHashCode(INode obj)
        {
            unchecked
            {
                var hashCode = (obj.ChildNodes != null ? obj.ChildNodes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Id;
                hashCode = (hashCode * 397) ^ obj.IsAreaNode.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsIterationNode.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ParentNode != null ? obj.ParentNode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Path != null ? obj.Path.GetHashCode() : 0);

                return hashCode;
            }
        }

        private class Nested
        {
            internal static readonly NodeComparer Instance = new NodeComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}