using System.Linq;

namespace Qwiq.Client.Soap
{
    internal static class LinkTypeEndMapper
    {
        internal static Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemLinkTypeEnd Map(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore store, IWorkItemLinkTypeEnd end)
        {
            var linkType = store.WorkItemLinkTypes.Single(type => type.ReferenceName == end.LinkType.ReferenceName);
            return end.IsForwardLink ? linkType.ForwardEnd : linkType.ReverseEnd;
        }
    }
}

