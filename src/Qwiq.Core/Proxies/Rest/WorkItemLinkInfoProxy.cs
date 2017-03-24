using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly WorkItemLink _item;

        internal WorkItemLinkInfoProxy(WorkItemLink item, int id)
        {
            _item = item;
            LinkTypeId = id;
        }

        public bool IsLocked { get; }

        public int LinkTypeId { get; }

        public int SourceId => (_item?.Source?.Id).GetValueOrDefault();

        public int TargetId => (_item?.Target?.Id).GetValueOrDefault();

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