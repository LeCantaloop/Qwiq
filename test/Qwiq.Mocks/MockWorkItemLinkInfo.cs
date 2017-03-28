namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkInfo : Microsoft.Qwiq.Proxies.WorkItemLinkInfoProxy
    {
        public MockWorkItemLinkInfo()
            : base(0)
        {
        }

        public MockWorkItemLinkInfo(int sourceId, int targetId)
            : this(sourceId, targetId, 0)
        {
        }

        public MockWorkItemLinkInfo(int sourceId, int targetId, int linkTypeId)
            : base(linkTypeId)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }
    }
}
