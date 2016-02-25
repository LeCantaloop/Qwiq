using System;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    internal class LinkHelper
    {
        public Tfs.Link FindEquivalentLink(Tfs.WorkItem item, ILink link)
        {
            var relatedLink = link as IRelatedLink;
            if (relatedLink != null)
            {
                return item.Links.Cast<Tfs.Link>().OfType<Tfs.RelatedLink>().SingleOrDefault(rl => rl.RelatedWorkItemId == relatedLink.RelatedWorkItemId);
            }

            var hyperlink = link as IHyperlink;
            if (hyperlink != null)
            {
                return item.Links.Cast<Tfs.Link>().OfType<Tfs.Hyperlink>().SingleOrDefault(hl => hl.Location == hyperlink.Location);
            }

            var workItemLink = link as IWorkItemLink;
            if (workItemLink != null)
            {
                return item
                        .Links
                        .Cast<Tfs.Link>()
                        .OfType<Tfs.WorkItemLink>()
                        .SingleOrDefault(wil => wil.LinkTypeEnd.ImmutableName.Equals(workItemLink.LinkTypeEnd.ImmutableName, StringComparison.OrdinalIgnoreCase)
                                                    && wil.SourceId == workItemLink.SourceId
                                                    && wil.TargetId == workItemLink.TargetId);
            }

            throw new ArgumentException("Unknown link type", "link");
        }
    }
}