using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Hyperlink : Link, IHyperlink
    {
        private readonly Tfs.Hyperlink _hyperLink;

        internal Hyperlink(Tfs.Hyperlink hyperLink)
            : base(hyperLink)
        {
            _hyperLink = hyperLink;
        }

        public string Location => _hyperLink.Location;
    }
}