using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkTypeEndProxy
    {
        public WorkItemLinkTypeEndProxy(WorkItemRelationType item)
        {
            ImmutableName = item.ReferenceName;
            Name = item.Name;
        }
    }
}