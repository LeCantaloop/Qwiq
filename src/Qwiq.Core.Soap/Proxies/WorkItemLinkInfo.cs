using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
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