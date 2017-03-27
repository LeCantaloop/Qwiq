using System.Collections.Generic;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkInfoComparer : GenericComparer<IWorkItemLinkInfo>
    {
        public static WorkItemLinkInfoComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItemLinkInfo x, IWorkItemLinkInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.IsLocked == y.IsLocked
                   && x.LinkTypeId == y.LinkTypeId
                   && x.SourceId == y.SourceId
                   && x.TargetId == y.TargetId;
        }

        public override int GetHashCode(IWorkItemLinkInfo obj)
        {
            unchecked
            {
                return 397 * (obj.LinkTypeId ^ obj.SourceId ^ obj.TargetId);
            }
        }

        private class Nested
        {
            internal static readonly WorkItemLinkInfoComparer Instance = new WorkItemLinkInfoComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}