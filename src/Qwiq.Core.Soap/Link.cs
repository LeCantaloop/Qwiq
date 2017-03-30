using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    public class Link : ILink
    {
        private readonly Tfs.Link _link;

        internal Link(Tfs.Link link)
        {
            _link = link;
        }

        public string Comment => _link.Comment;

        public BaseLinkType BaseType => (BaseLinkType) _link.BaseType;
    }
}

