using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class HyperlinkProxy : LinkProxy, IHyperlink
    {
        private readonly Tfs.Hyperlink _hyperLink;

        internal HyperlinkProxy(Tfs.Hyperlink hyperLink) : base(hyperLink)
        {
            _hyperLink = hyperLink;
        }

        public string Location
        {
            get { return _hyperLink.Location; }
        }
    }
}

