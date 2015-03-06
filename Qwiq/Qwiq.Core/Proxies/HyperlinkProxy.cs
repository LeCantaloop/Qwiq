using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class HyperlinkProxy : IHyperlink
    {
        private readonly Tfs.Hyperlink _hyperLink;

        internal HyperlinkProxy(Tfs.Hyperlink hyperLink)
        {
            _hyperLink = hyperLink;
        }

        public string Location
        {
            get { return _hyperLink.Location; }
        }

        public string Comment
        {
            get { return _hyperLink.Comment; }
        }
    }
}
