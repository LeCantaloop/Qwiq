using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    public class RelatedLink : Link, IRelatedLink
    {
        private readonly Tfs.RelatedLink _relatedLink;

        internal RelatedLink(Tfs.RelatedLink relatedLink) : base(relatedLink)
        {
            _relatedLink = relatedLink;
        }

        public int RelatedWorkItemId => _relatedLink.RelatedWorkItemId;

        public IWorkItemLinkTypeEnd LinkTypeEnd => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEnd(_relatedLink.LinkTypeEnd));

        public string LinkSubType => _relatedLink.LinkTypeEnd.Name;
    }
}

