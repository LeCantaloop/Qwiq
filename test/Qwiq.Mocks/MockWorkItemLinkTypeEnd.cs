using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkTypeEnd : IWorkItemLinkTypeEnd
    {
        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes (IWorkItemLinkType, String).")]
        public MockWorkItemLinkTypeEnd(string name)
        {
            Name = name;
        }

        public MockWorkItemLinkTypeEnd(IWorkItemLinkType linkType, string name, int id = 0)
        {
            Id = id;
            LinkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Id { get; }

        public string ImmutableName

        {
            get
            {
                var referenceName = LinkType.ReferenceName;
                if (!LinkType.IsDirectional)
                {
                    return referenceName;
                }
                return referenceName + (IsForwardLink ? "-Forward" : "-Reverse");
            }
        }

        public bool IsForwardLink => Equals(LinkType.ForwardEnd, this);

        public IWorkItemLinkType LinkType { get; }

        public string Name { get; }

        public IWorkItemLinkTypeEnd OppositeEnd => !IsForwardLink ? LinkType.ForwardEnd : LinkType.ReverseEnd;
    }
}
