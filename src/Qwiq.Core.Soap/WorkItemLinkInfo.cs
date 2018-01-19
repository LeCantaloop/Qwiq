using JetBrains.Annotations;

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemLinkInfo : Qwiq.WorkItemLinkInfo, IIdentifiable<int>
    {
        /// <inheritdoc />
        internal WorkItemLinkInfo(int sourceId, int targetId, int linkTypeId, [CanBeNull] IWorkItemLinkTypeEnd linkTypeEnd)
            : base(sourceId, targetId, linkTypeEnd)
        {
            Id = linkTypeId;
        }

        public int Id { get; }
    }
}