using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
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
        
        public string Comment
        {
            get { return _relatedLink.Comment; }
        }
    }
}
