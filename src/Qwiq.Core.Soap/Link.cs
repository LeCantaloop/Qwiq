using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Link : ILink
    {
        private readonly Tfs.Link _link;

        internal Link(Tfs.Link link)
        {
            _link = link ?? throw new ArgumentNullException(nameof(link));
        }

        public string Comment => _link.Comment;

        public BaseLinkType BaseType => (BaseLinkType) _link.BaseType;
    }
}

