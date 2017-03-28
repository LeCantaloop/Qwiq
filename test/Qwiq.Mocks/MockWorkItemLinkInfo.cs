namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkInfo : IWorkItemLinkInfo
    {
        public MockWorkItemLinkInfo()
        {
        }

        public MockWorkItemLinkInfo(int sourceId, int targetId)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }

        public bool IsLocked { get; set; }
        public int LinkTypeId { get; set; }
        public int SourceId { get; set; }
        public int TargetId { get; set; }

        public static bool operator !=(MockWorkItemLinkInfo x, MockWorkItemLinkInfo y)
        {
            return !WorkItemLinkInfoComparer.Instance.Equals(x, y);
        }

        public static bool operator ==(MockWorkItemLinkInfo x, MockWorkItemLinkInfo y)
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
    }
}
