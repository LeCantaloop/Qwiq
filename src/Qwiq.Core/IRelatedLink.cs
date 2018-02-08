using System;

namespace Qwiq
{
    public interface IRelatedLink : ILink, IEquatable<IRelatedLink>
    {
        IWorkItemLinkTypeEnd LinkTypeEnd { get; }

        int RelatedWorkItemId { get; }
    }
}