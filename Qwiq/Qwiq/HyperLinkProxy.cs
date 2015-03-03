using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class HyperLinkProxy : IHyperLink
    {
        private readonly Tfs.Hyperlink _hyperLink;

        internal HyperLinkProxy(Tfs.Hyperlink hyperLink)
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
