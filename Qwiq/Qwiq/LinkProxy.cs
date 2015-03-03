using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class LinkProxy : ILink
    {
        private readonly Tfs.Link _link;

        public LinkProxy(Tfs.Link link)
        {
            _link = link;
        }

        public string Comment
        {
            get { return _link.Comment; }
        }
    }
}
