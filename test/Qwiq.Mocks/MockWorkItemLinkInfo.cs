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
    }
}
