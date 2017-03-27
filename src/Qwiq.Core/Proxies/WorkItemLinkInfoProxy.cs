using System;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly Lazy<int> _id;

        private WorkItemLinkInfoProxy(Lazy<int> id)
        {
            _id = id;
        }

        public bool IsLocked { get; }

        public int LinkTypeId => _id.Value;

        public int SourceId { get; }

        public int TargetId { get; }

        public static bool operator !=(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return !WorkItemLinkInfoComparer.Instance.Equals(x, y);
        }

        public static bool operator ==(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return WorkItemLinkInfoComparer.Instance.Equals(x, y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IWorkItemLinkInfo)) return false;
            return WorkItemLinkInfoComparer.Instance.Equals(this, (IWorkItemLinkInfo)obj);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkInfoComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"S:{SourceId} T:{TargetId} ID:{LinkTypeId}";
        }
    }
}