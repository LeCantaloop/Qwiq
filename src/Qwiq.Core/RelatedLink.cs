using System.Diagnostics;

using JetBrains.Annotations;

namespace Qwiq
{
    public class RelatedLink : Link, IRelatedLink
    {
        internal RelatedLink(int related, [CanBeNull] IWorkItemLinkTypeEnd linkTypeEnd = null, [CanBeNull] string comment = null)
            : base(comment, BaseLinkType.RelatedLink)
        {
            RelatedWorkItemId = related;

            LinkTypeEnd = linkTypeEnd;
        }

        public IWorkItemLinkTypeEnd LinkTypeEnd { get; }

        public int RelatedWorkItemId { get; }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return Equals(obj as IRelatedLink);
        }

        public bool Equals(IRelatedLink other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return RelatedWorkItemId.Equals(other.RelatedWorkItemId)
                   && WorkItemLinkTypeEndComparer.Default.Equals(LinkTypeEnd, other.LinkTypeEnd);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ RelatedWorkItemId.GetHashCode();
                hash = (13 * hash) ^ (LinkTypeEnd != null ? WorkItemLinkTypeEndComparer.Default.GetHashCode(LinkTypeEnd) : 0);

                return hash;
            }
        }
    }
}