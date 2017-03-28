namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkInfo : WorkItemLinkInfo
    {
        public MockWorkItemLinkInfo(int sourceId, int targetId)
            : this(sourceId, targetId, 0)
        {
        }

        public MockWorkItemLinkInfo(int sourceId, int targetId, int linkTypeId)
            : base(sourceId, targetId, linkTypeId)
        {
        }
    }
}
