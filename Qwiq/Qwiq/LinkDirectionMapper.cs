using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    internal static class LinkDirectionMapper
    {
        public static WorkItemLinkTypeEnd Map(WorkItemStore store, WorkItemLinkDirection linkDirection)
        {
            var linkType = store.WorkItemLinkTypes.Single(type => type.ReferenceName == "System.LinkTypes.Hierarchy");
            
            return linkDirection == WorkItemLinkDirection.Forward ? linkType.ForwardEnd : linkType.ReverseEnd;
        }

        public static WorkItemLinkDirection Map(WorkItemLinkTypeEnd linkTypeEnd)
        {
            return linkTypeEnd.IsForwardLink ? WorkItemLinkDirection.Forward : WorkItemLinkDirection.Reverse;
        }
    }
}
