using System;

using JetBrains.Annotations;
#pragma warning disable 618

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemLinkInfo : Qwiq.WorkItemLinkInfo
    {
        /// <inheritdoc />
        internal WorkItemLinkInfo(int sourceId, int targetId, int linkTypeId, [CanBeNull] IWorkItemLinkTypeEnd linkTypeEnd)
            : base(sourceId, targetId, linkTypeEnd)
        {
            LinkTypeId = linkTypeId;
        }

        [Obsolete("This property is deprecated and will be removed in a future release.")]
        public int LinkTypeId { get; }
    }
}
