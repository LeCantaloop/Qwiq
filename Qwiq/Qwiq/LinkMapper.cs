using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    internal class LinkMapper
    {
        public Tfs.Link Map(ILink link, Tfs.WorkItem item)
        {
            var relatedLink = link as IRelatedLink;
            if (relatedLink != null)
            {
                var linkTypeEnd = item.Store.WorkItemLinkTypes.LinkTypeEnds[relatedLink.LinkTypeEnd.ImmutableName];
                return new Tfs.RelatedLink(linkTypeEnd, relatedLink.RelatedWorkItemId);
            }

            var hyperlink = link as IHyperlink;
            if (hyperlink != null)
            {
                return new Tfs.Hyperlink(hyperlink.Location);
            }

            throw new ArgumentException("Unknown link type", "link");
        }
    }
}