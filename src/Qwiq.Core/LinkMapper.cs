using System;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Microsoft.Qwiq.Proxies.Soap;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq
{
    internal class LinkMapper
    {
        public Tfs.Link Map(ILink link, Tfs.WorkItem item)
        {
            if (link.BaseType == BaseLinkType.RelatedLink)
            {
                var relatedLink = (IRelatedLink)link;
                var linkTypeEnd = LinkTypeEndMapper.Map(item.Store, relatedLink.LinkTypeEnd);
                return new Tfs.RelatedLink(linkTypeEnd, relatedLink.RelatedWorkItemId);
            }
            if (link.BaseType == BaseLinkType.Hyperlink)
            {
                var hyperlink = (IHyperlink) link;
                return new Tfs.Hyperlink(hyperlink.Location);
            }
            throw new ArgumentException("Unknown link type", nameof(link));
        }

        public ILink Map(Tfs.Link link)
        {
            if (link.BaseType == Tfs.BaseLinkType.RelatedLink)
            {
                var relatedLink = (Tfs.RelatedLink) link;
                return ExceptionHandlingDynamicProxyFactory.Create<IRelatedLink>(new RelatedLinkProxy(relatedLink));

            }
            if (link.BaseType == Tfs.BaseLinkType.Hyperlink)
            {
                var hyperlink = (Tfs.Hyperlink) link;
                return ExceptionHandlingDynamicProxyFactory.Create<IHyperlink>(new HyperlinkProxy(hyperlink));
            }

            throw new ArgumentException("Unknown link type", nameof(link));
        }
    }
}

