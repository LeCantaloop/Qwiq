using System;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkTypeEndComparer : GenericComparer<IWorkItemLinkTypeEnd>
    {
        public static WorkItemLinkTypeEndComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItemLinkTypeEnd x, IWorkItemLinkTypeEnd y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.IsForwardLink == y.IsForwardLink
                   && x.ImmutableName.Equals(y.ImmutableName, StringComparison.OrdinalIgnoreCase)
                   && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(IWorkItemLinkTypeEnd obj)
        {
            unchecked
            {
                return 397 * (obj.Id ^ obj.ImmutableName.GetHashCode());
            }
        }

        private class Nested
        {
            internal static readonly WorkItemLinkTypeEndComparer Instance = new WorkItemLinkTypeEndComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}