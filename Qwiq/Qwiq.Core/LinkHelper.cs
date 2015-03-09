using System;
using System.Linq;
using Microsoft.IE.Qwiq.Proxies;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    internal class LinkHelper
    {
        public Tfs.Link Map(ILink link, Tfs.WorkItem item)
        {
            var relatedLink = link as IRelatedLink;
            if (relatedLink != null)
            {
                var linkTypeEnd = LinkDirectionMapper.Map(item.Store, relatedLink.LinkDirection);
                return new Tfs.RelatedLink(linkTypeEnd, relatedLink.RelatedWorkItemId);
            }

            var hyperlink = link as IHyperlink;
            if (hyperlink != null)
            {
                return new Tfs.Hyperlink(hyperlink.Location);
            }

            var workItemLink = link as IWorkItemLink;
            if (workItemLink != null)
            {
                var linkTypeEnd = LinkDirectionMapper.Map(item.Store, workItemLink.LinkDirection);
                return new Tfs.WorkItemLink(linkTypeEnd, workItemLink.SourceId, workItemLink.TargetId);
            }

            throw new ArgumentException("Unknown link type", "link");
        }

        public ILink Map(Tfs.Link link, Tfs.WorkItem item)
        {
            var relatedLink = link as Tfs.RelatedLink;
            if (relatedLink != null)
            {
                return new RelatedLinkProxy(relatedLink);
            }

            var hyperlink = link as Tfs.Hyperlink;
            if (hyperlink != null)
            {
                return new HyperlinkProxy(hyperlink);
            }

            var workItemLink = link as Tfs.WorkItemLink;
            if (workItemLink != null)
            {
                return new WorkItemLinkProxy(workItemLink);
            }

            throw new ArgumentException("Unknown link type", "link");
        }

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
                return item.Links.Cast<Tfs.Link>().OfType<Tfs.WorkItemLink>().SingleOrDefault(wil => wil.SourceId == workItemLink.SourceId && wil.TargetId == workItemLink.TargetId);
            }
        
            throw new ArgumentException("Unknown link type", "link");
        } 
    }
}