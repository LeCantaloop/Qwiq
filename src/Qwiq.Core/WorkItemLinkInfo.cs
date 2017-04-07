using System;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkInfo : IWorkItemLinkInfo, IEquatable<IWorkItemLinkInfo>
    {
        private readonly Lazy<int> _id;

        internal WorkItemLinkInfo(int sourceId, int targetId, int id)
            : this(sourceId, targetId, new Lazy<int>(() => id))
        {

        }

        internal WorkItemLinkInfo(int sourceId, int targetId, Lazy<int> id)
        {
            SourceId = sourceId;
            TargetId = targetId;
            _id = id;
        }

        public bool Equals(IWorkItemLinkInfo other)
        {
            return WorkItemLinkInfoComparer.Instance.Equals(this, other);
        }

        public bool IsLocked { get; internal set; }

        public int LinkTypeId => _id.Value;

        public int SourceId { get; }

        public int TargetId { get; }

        public static bool operator !=(WorkItemLinkInfo x, WorkItemLinkInfo y)
        {
            return !WorkItemLinkInfoComparer.Instance.Equals(x, y);
        }

        public static bool operator ==(WorkItemLinkInfo x, WorkItemLinkInfo y)
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