namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkInfo : WorkItemLinkInfo
    {
        public MockWorkItemLinkInfo(int sourceId, int targetId)
            : this(sourceId, targetId, CoreLinkTypes.Related)
        {
        }

        public MockWorkItemLinkInfo(int sourceId, int targetId, int linkTypeId)
            : base(sourceId, targetId, linkTypeId)
        {
        }
    }
}
