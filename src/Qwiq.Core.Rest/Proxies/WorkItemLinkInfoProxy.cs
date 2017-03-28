using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class WorkItemLinkInfoProxy : Microsoft.Qwiq.Proxies.WorkItemLinkInfoProxy
    {
        internal WorkItemLinkInfoProxy(WorkItemLink item, Lazy<int> id)
            : base(id)
        {
            IsLocked = false;
            SourceId = item.Source?.Id ?? 0;
            TargetId = item.Target?.Id ?? 0;
        }
    }
}