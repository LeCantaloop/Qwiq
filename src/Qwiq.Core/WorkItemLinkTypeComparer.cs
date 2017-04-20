using System;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeComparer : GenericComparer<IWorkItemLinkType>
    {
        internal new static WorkItemLinkTypeComparer Default => Nested.Instance;

        private WorkItemLinkTypeComparer()
        {
        }

        public override bool Equals(IWorkItemLinkType x, IWorkItemLinkType y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.IsActive == y.IsActive
                && x.IsDirectional == y.IsDirectional
                && string.Equals(x.ReferenceName, y.ReferenceName, StringComparison.OrdinalIgnoreCase)
                && WorkItemLinkTypeEndComparer.Default.Equals(x.ForwardEnd, y.ForwardEnd)
                && WorkItemLinkTypeEndComparer.Default.Equals(x.ReverseEnd, y.ReverseEnd);
        }

        public override int GetHashCode(IWorkItemLinkType obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.ReferenceName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ReferenceName) : 0);
                hash = (13 * hash) ^ obj.IsActive.GetHashCode();
                hash = (13 * hash) ^ obj.IsDirectional.GetHashCode();
                hash = (13 * hash) ^ (obj.ForwardEnd != null ? WorkItemLinkTypeEndComparer.Default.GetHashCode(obj.ForwardEnd) : 0);
                hash = (13 * hash) ^ (obj.ReverseEnd != null ? WorkItemLinkTypeEndComparer.Default.GetHashCode(obj.ReverseEnd) : 0);

                return hash;
            }
        }

        private class Nested
        {
            internal static readonly WorkItemLinkTypeComparer Instance = new WorkItemLinkTypeComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}