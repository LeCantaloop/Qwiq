using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkTypeEnd : Microsoft.Qwiq.Proxies.WorkItemLinkTypeEndProxy
    {
        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes (IWorkItemLinkType, String, Bool, Int32).")]
        public MockWorkItemLinkTypeEnd(string name)
        {
            Name = name;
        }

        public MockWorkItemLinkTypeEnd(IWorkItemLinkType linkType, string name, bool isForward, int id = 0)
        {
            Id = id;
            LinkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsForwardLink = isForward;

            var referenceName = LinkType.ReferenceName;
            if (!LinkType.IsDirectional)
            {
                ImmutableName = referenceName;
            }
            ImmutableName = referenceName + (IsForwardLink ? "-Forward" : "-Reverse");
        }
    }
}
