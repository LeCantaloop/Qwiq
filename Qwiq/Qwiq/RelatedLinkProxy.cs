using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class RelatedLinkProxy : IRelatedLink
    {
        private readonly Tfs.RelatedLink _relatedLink;

        public RelatedLinkProxy(IWorkItemLinkTypeEnd linkTypeEnd, int relatedWorkItemId)
        {
            _relatedLink = new Tfs.RelatedLink((linkTypeEnd as WorkItemLinkTypeEndProxy).End ,relatedWorkItemId);
        }

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
            get { return new WorkItemLinkTypeEndProxy(_relatedLink.LinkTypeEnd);  }
        }

        public string Comment
        {
            get { return _relatedLink.Comment; }
        }
    }
}
