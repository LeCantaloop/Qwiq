using System;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkTypeComparer : GenericComparer<IWorkItemLinkType>
    {
        public static WorkItemLinkTypeComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItemLinkType x, IWorkItemLinkType y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.IsActive == y.IsActive
                   && x.ReferenceName.Equals(y.ReferenceName, StringComparison.OrdinalIgnoreCase)
                   && WorkItemLinkTypeEndComparer.Instance.Equals(x.ForwardEnd, y.ForwardEnd)
                   && WorkItemLinkTypeEndComparer.Instance.Equals(x.ReverseEnd, y.ReverseEnd);
        }

        public override int GetHashCode(IWorkItemLinkType obj)
        {
            unchecked
            {
                return 397 * (obj.ForwardEnd.GetHashCode() ^ obj.ReverseEnd.GetHashCode()
                              ^ obj.ReferenceName.GetHashCode());
            }
        }

        private class Nested
        {
            internal static readonly WorkItemLinkTypeComparer Instance = new WorkItemLinkTypeComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}