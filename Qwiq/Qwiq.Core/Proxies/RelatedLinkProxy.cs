using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
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
                return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEndProxy(_relatedLink.LinkTypeEnd));
            }
        }

        public string LinkSubType
        {
            get { return _relatedLink.LinkTypeEnd.Name;  }
        }
    }
}
