using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly Tfs.WorkItemLinkInfo _item;

        internal WorkItemLinkInfoProxy(Tfs.WorkItemLinkInfo item)
        {
            _item = item;
        }

        public bool IsLocked => _item.IsLocked;

        public int LinkTypeId => _item.LinkTypeId;

        public int SourceId => _item.SourceId;

        public int TargetId => _item.TargetId;

        public static bool operator !=(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return !WorkItemLinkInfoEqualityComparer.Instance.Equals(x, y);
        }

        public static bool operator ==(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return WorkItemLinkInfoEqualityComparer.Instance.Equals(x, y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IWorkItemLinkInfo)) return false;
            return WorkItemLinkInfoEqualityComparer.Instance.Equals(this, (IWorkItemLinkInfo)obj);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkInfoEqualityComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"S:{SourceId} T:{TargetId} ID:{LinkTypeId}";
        }
    }
}