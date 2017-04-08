using System.Linq;

namespace Microsoft.Qwiq
{
    public class NodeCollectionComparer : GenericComparer<INodeCollection>
    {
        public static readonly NodeCollectionComparer Instance = Nested.Instance;

        private NodeCollectionComparer()
        {
        }

        public override bool Equals(INodeCollection x, INodeCollection y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            if (x.Count != y.Count) return false;

            var expected = x.ToList();
            var source = y.ToList();
            foreach (var wit in expected)
            {
                if (!source.Contains(wit, NodeComparer.Instance)) return false;

                // Removes the first occurrence, so if there are duplicates we'll still get a valid mismatch
                source.Remove(wit);
            }

            // If there are any items left then fail
            if (source.Any()) return false;

            foreach (var wit in expected)
            {
                var sw = wit;
                var tw = y[sw.Name];

                if (NodeComparer.Instance.Equals(sw, tw)) return false;
            }

            return true;
        }

        public override int GetHashCode(INodeCollection obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            // IMPORTANT: The collection must be in the same order to produce the same hash
            var hash = 27;
            foreach (var node in obj.OrderBy(p => p.Name)) hash = (13 * hash) ^ node.GetHashCode();
            return hash;
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly NodeCollectionComparer Instance = new NodeCollectionComparer();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}