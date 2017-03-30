using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class WorkItemLinkInfo : Qwiq.WorkItemLinkInfo
    {
        internal WorkItemLinkInfo(Tfs.WorkItemLinkInfo item)
            : base(item.SourceId, item.TargetId, item.LinkTypeId)
        {
            IsLocked = item.IsLocked;
        }
    }
}