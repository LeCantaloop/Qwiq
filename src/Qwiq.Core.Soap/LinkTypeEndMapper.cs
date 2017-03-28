using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal static class LinkTypeEndMapper
    {
        public static TeamFoundation.WorkItemTracking.Client.WorkItemLinkTypeEnd Map(WorkItemStore store, IWorkItemLinkTypeEnd end)
        {
            var linkType = store.WorkItemLinkTypes.Single(type => type.ReferenceName == end.LinkType.ReferenceName);
            return end.IsForwardLink ? linkType.ForwardEnd : linkType.ReverseEnd;
        }
    }
}

