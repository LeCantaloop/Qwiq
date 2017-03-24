using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeEndEqualityComparer : GenericComparer<IWorkItemLinkTypeEnd>
    {
        public static IEqualityComparer<IWorkItemLinkTypeEnd> Instance => Nested.Instance;

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
            internal static readonly IEqualityComparer<IWorkItemLinkTypeEnd> Instance = new WorkItemLinkTypeEndEqualityComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}