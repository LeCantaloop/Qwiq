using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class WorkItemLinkInfoProxy : Microsoft.Qwiq.Proxies.WorkItemLinkInfoProxy
    {
        internal WorkItemLinkInfoProxy(Tfs.WorkItemLinkInfo item)
            : base(new Lazy<int>(() => item.LinkTypeId))
        {
            IsLocked = item.IsLocked;
            SourceId = item.SourceId;
            TargetId = item.TargetId;
        }
    }
}