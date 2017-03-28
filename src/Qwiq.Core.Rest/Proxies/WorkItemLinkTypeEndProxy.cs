using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal partial class WorkItemLinkTypeEndProxy : Microsoft.Qwiq.Proxies.WorkItemLinkTypeEndProxy
    {
        internal WorkItemLinkTypeEndProxy(WorkItemRelationType item)
        {
            ImmutableName = item.ReferenceName;
            Name = item.Name;
        }
    }
}