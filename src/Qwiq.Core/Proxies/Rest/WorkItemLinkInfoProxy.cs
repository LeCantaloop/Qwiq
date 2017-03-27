using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkInfoProxy
    {
        internal WorkItemLinkInfoProxy(WorkItemLink item, Lazy<int> id)
            : this(id)
        {
            IsLocked = false;
            SourceId = item.Source?.Id ?? 0;
            TargetId = item.Target?.Id ?? 0;
        }
    }
}