using System;
using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class LinkHelper
    {
        internal Tfs.Link FindEquivalentLink(Tfs.WorkItem item, ILink link)
        {
            if (link.BaseType == BaseLinkType.RelatedLink)
            {
                var relatedLink = (IRelatedLink) link;
                return
                    item.Links.Cast<Tfs.Link>()
                        .OfType<Tfs.RelatedLink>()
                        .SingleOrDefault(
                            rl =>
                                rl.LinkTypeEnd.ImmutableName.Equals(relatedLink.LinkTypeEnd.ImmutableName, StringComparison.OrdinalIgnoreCase)
                                && rl.RelatedWorkItemId == relatedLink.RelatedWorkItemId);
            }
            if (link.BaseType == BaseLinkType.Hyperlink)
            {
                var hyperlink = (IHyperlink) link;
                return
                    item.Links.Cast<Tfs.Link>()
                        .OfType<Tfs.Hyperlink>()
                        .SingleOrDefault(hl => hl.Location == hyperlink.Location);
            }

            throw new ArgumentException("Unknown link type", nameof(link));
        }
    }
}
