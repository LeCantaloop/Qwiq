using System;

namespace Qwiq.Mocks
{
    public class MockWorkItemLinkTypeEnd : WorkItemLinkTypeEnd, IIdentifiable<int>
    {
        public MockWorkItemLinkTypeEnd(IWorkItemLinkType linkType, string name, bool isForward, int id = 0)
            : base(GetValue(linkType, isForward))
        {
            Id = id;
            LinkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsForwardLink = isForward;
        }

        private static string GetValue(IWorkItemLinkType linkType, bool isForward)
        {
            if (linkType == null) throw new ArgumentNullException(nameof(linkType));
            var referenceName = linkType.ReferenceName;
            if (!linkType.IsDirectional) return referenceName;
            return referenceName + (isForward ? "-Forward" : "-Reverse");
        }

        /// <inheritdoc />
        public int Id { get; }
    }
}