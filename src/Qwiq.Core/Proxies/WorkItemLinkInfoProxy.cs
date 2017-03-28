using System;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo, IEquatable<IWorkItemLinkInfo>, IComparable<IWorkItemLinkInfo>
    {
        private readonly Lazy<int> _id;

        internal WorkItemLinkInfoProxy(int id)
            :this(new Lazy<int>(()=>id))
        {

        }

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

        public int CompareTo(IWorkItemLinkInfo other)
        {
            return WorkItemLinkInfoComparer.Instance.Compare(this, other);
        }

        public override string ToString()
        {
            return $"S:{SourceId} T:{TargetId} ID:{LinkTypeId}";
        }
    }
}