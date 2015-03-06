using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class LinkProxy : ILink
    {
        private readonly Tfs.Link Link;

        internal LinkProxy(Tfs.Link link)
        {
            Link = link;
        }

        public string Comment
        {
            get { return Link.Comment; }
        }
    }
}
