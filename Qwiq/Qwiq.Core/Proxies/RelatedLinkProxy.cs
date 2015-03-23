using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class RelatedLinkProxy : IRelatedLink
    {
        private readonly Tfs.RelatedLink _relatedLink;

        internal RelatedLinkProxy(Tfs.RelatedLink relatedLink)
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
                return new WorkItemLinkTypeEndProxy(_relatedLink.LinkTypeEnd);
            }
        }

        public string LinkSubType
        {
            get { return _relatedLink.LinkTypeEnd.Name;  }
        }

        public string Comment
        {
            get { return _relatedLink.Comment; }
        }
    }
}
