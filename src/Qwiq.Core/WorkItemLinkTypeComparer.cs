using System;

namespace Microsoft.Qwiq
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
                && x.IsDirectional == y.IsDirectional
                && string.Equals(x.ReferenceName, y.ReferenceName, StringComparison.OrdinalIgnoreCase)
                && Equals(x.ForwardEnd, y.ForwardEnd)
                && Equals(x.ReverseEnd, y.ReverseEnd);
        }

        public override int GetHashCode(IWorkItemLinkType obj)
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.ReferenceName != null ? obj.ReferenceName.GetHashCode() : 0);
                hash = (13 * hash) ^ obj.IsActive.GetHashCode();
                hash = (13 * hash) ^ obj.IsDirectional.GetHashCode();
                hash = (13 * hash) ^ (obj.ForwardEnd != null ? obj.ForwardEnd.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.ReverseEnd != null ? obj.ReverseEnd.GetHashCode() : 0);

                return hash;
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