using System;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.IE.Qwiq.Proxies;
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
                var linkTypeEnd = LinkTypeEndMapper.Map(item.Store, relatedLink.LinkTypeEnd);
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
                var linkTypeEnd = LinkTypeEndMapper.Map(item.Store, workItemLink.LinkTypeEnd);
                return new Tfs.WorkItemLink(linkTypeEnd, workItemLink.SourceId, workItemLink.TargetId);
            }

            throw new ArgumentException("Unknown link type", "link");
        }

        public ILink Map(Tfs.Link link)
        {
            var relatedLink = link as Tfs.RelatedLink;
            if (relatedLink != null)
            {
                return ExceptionHandlingDynamicProxyFactory.Create<IRelatedLink>(new RelatedLinkProxy(relatedLink));
            }

            var hyperlink = link as Tfs.Hyperlink;
            if (hyperlink != null)
            {
                return ExceptionHandlingDynamicProxyFactory.Create<IHyperlink>(new HyperlinkProxy(hyperlink));
            }

            var workItemLink = link as Tfs.WorkItemLink;
            if (workItemLink != null)
            {
                return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLink>(new WorkItemLinkProxy(workItemLink));
            }

            throw new ArgumentException("Unknown link type", "link");
        }
    }
}
