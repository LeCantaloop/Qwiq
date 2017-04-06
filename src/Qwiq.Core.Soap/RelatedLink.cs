using System;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class RelatedLink : Link, IRelatedLink
    {
        private readonly Tfs.RelatedLink _relatedLink;

        private readonly Lazy<IWorkItemLinkTypeEnd> _linkTypeEnd;

        internal RelatedLink(Tfs.RelatedLink relatedLink) : base(relatedLink)
        {
            _relatedLink = relatedLink;
            _linkTypeEnd = new Lazy<IWorkItemLinkTypeEnd>(()=> ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEnd(_relatedLink.LinkTypeEnd)));
        }

        public int RelatedWorkItemId => _relatedLink.RelatedWorkItemId;

        public IWorkItemLinkTypeEnd LinkTypeEnd => _linkTypeEnd.Value;

        public string LinkSubType => _relatedLink.LinkTypeEnd.Name;
    }
}

