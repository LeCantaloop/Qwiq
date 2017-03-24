using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkInfoEqualityComparer : IEqualityComparer<IWorkItemLinkInfo>
    {
        public static IEqualityComparer<IWorkItemLinkInfo> Instance => Nested.Instance;

        public bool Equals(IWorkItemLinkInfo x, IWorkItemLinkInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            var x1 = x as IEquatable<IWorkItemLinkInfo>;
            var y1 = y as IEquatable<IWorkItemLinkInfo>;

            if (x1 != null && y1 != null)
            {
                return x1.Equals(y1);
            }

            return x.IsLocked == y.IsLocked &&
                   x.LinkTypeId == y.LinkTypeId &&
                   x.SourceId == y.SourceId &&
                   x.TargetId == y.TargetId;
        }

        public int GetHashCode(IWorkItemLinkInfo obj)
        {
            unchecked
            {
                return 397 * (obj.LinkTypeId ^ obj.SourceId ^ obj.TargetId);
            }
        }

        private class Nested
        {
            internal static readonly IEqualityComparer<IWorkItemLinkInfo> Instance = new WorkItemLinkInfoEqualityComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }


}