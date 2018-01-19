using System;

namespace Microsoft.Qwiq
{
    public interface IRelatedLink : ILink, IEquatable<IRelatedLink>
    {
        IWorkItemLinkTypeEnd LinkTypeEnd { get; }

        int RelatedWorkItemId { get; }
    }
}