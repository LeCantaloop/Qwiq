using System.Linq;

namespace Microsoft.Qwiq.Client.Soap
{
    internal static class LinkTypeEndMapper
    {
        internal static TeamFoundation.WorkItemTracking.Client.WorkItemLinkTypeEnd Map(TeamFoundation.WorkItemTracking.Client.WorkItemStore store, IWorkItemLinkTypeEnd end)
        {
            var linkType = store.WorkItemLinkTypes.Single(type => type.ReferenceName == end.LinkType.ReferenceName);
            return end.IsForwardLink ? linkType.ForwardEnd : linkType.ReverseEnd;
        }
    }
}

