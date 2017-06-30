using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    internal class NodeComparer : GenericComparer<INode>
    {
        internal new static readonly NodeComparer Default = Nested.Instance;

        private NodeComparer()
        {
        }

        public override bool Equals(INode x, INode y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Id == y.Id
                && x.IsAreaNode == y.IsAreaNode
                && x.IsIterationNode == y.IsIterationNode
                && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode([CanBeNull] INode obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;

                hash = (hash * 13) ^ obj.Id;
                hash = (hash * 13) ^ obj.IsAreaNode.GetHashCode();
                hash = (hash * 13) ^ obj.IsIterationNode.GetHashCode();
                hash = (hash * 13) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (hash * 13) ^ (obj.Path != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Path) : 0);

                return hash;
            }
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly NodeComparer Instance = new NodeComparer();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}