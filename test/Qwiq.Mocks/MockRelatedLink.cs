using System;
using System.Diagnostics;

namespace Microsoft.Qwiq.Mocks
{
    public class MockRelatedLink : RelatedLink
    {
        [DebuggerStepThrough]
        public MockRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd)
            : base(0, linkTypeEnd)
        {
            LinkInfo = new WorkItemLinkInfo(0, 0, linkTypeEnd);
        }

        [DebuggerStepThrough]
        public MockRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, int targetId)
            : base(targetId, linkTypeEnd)
        {
            LinkInfo = new WorkItemLinkInfo(0, targetId, linkTypeEnd);
        }

        public MockRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, int sourceId, int targetId)
            : base(targetId, linkTypeEnd)
        {
            if (sourceId == targetId)
                throw new ArgumentException($"Parameter {nameof(sourceId)} cannot be the same as {nameof(targetId)}.");
            LinkInfo = new WorkItemLinkInfo(sourceId, targetId, linkTypeEnd);
        }

        internal IWorkItemLinkInfo LinkInfo { get; }
    }
}