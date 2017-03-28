using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    public class RelatedLinkProxy : LinkProxy, IRelatedLink
    {
        private readonly Tfs.RelatedLink _relatedLink;

        internal RelatedLinkProxy(Tfs.RelatedLink relatedLink) : base(relatedLink)
        {
            _relatedLink = relatedLink;
        }

        public int RelatedWorkItemId
        {
            get { return _relatedLink.RelatedWorkItemId; }
        }

        public IWorkItemLinkTypeEnd LinkTypeEnd
        {
            get
            {
                return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEnd(_relatedLink.LinkTypeEnd));
            }
        }

        public string LinkSubType
        {
            get { return _relatedLink.LinkTypeEnd.Name;  }
        }
    }
}

