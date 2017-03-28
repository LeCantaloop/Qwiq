using System;

namespace Microsoft.Qwiq.Proxies
{
    internal class WorkItemLinkInfoProxy : IWorkItemLinkInfo, IEquatable<IWorkItemLinkInfo>
    {
        private readonly Lazy<int> _id;

        internal WorkItemLinkInfoProxy(Lazy<int> id)
        {
            _id = id;
        }

        public bool Equals(IWorkItemLinkInfo other)
        {
            return WorkItemLinkInfoComparer.Instance.Equals(this, other);
        }

        public bool IsLocked { get; internal set; }

        public int LinkTypeId => _id.Value;

        public int SourceId { get; internal set; }

        public int TargetId { get; internal set; }

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
            return WorkItemLinkInfoComparer.Instance.Equals(this, obj as IWorkItemLinkInfo);
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