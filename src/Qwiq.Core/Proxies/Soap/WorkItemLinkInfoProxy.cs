using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkInfoProxy
    {
        private readonly Tfs.WorkItemLinkInfo _item;

        internal WorkItemLinkInfoProxy(Tfs.WorkItemLinkInfo item)
            : this(new Lazy<int>(() => item.LinkTypeId))
        {
            IsLocked = item.IsLocked;
            SourceId = item.SourceId;
            TargetId = item.TargetId;
        }
    }
}