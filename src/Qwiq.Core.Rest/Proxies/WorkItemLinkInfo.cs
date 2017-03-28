using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class WorkItemLinkInfo : Qwiq.WorkItemLinkInfo
    {
        internal WorkItemLinkInfo(WorkItemLink item, Lazy<int> id)
            : base(item.Source?.Id ?? 0, item.Target?.Id ?? 0, id)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            IsLocked = false;
        }
    }
}