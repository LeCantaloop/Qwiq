using System;

namespace Microsoft.Qwiq
{
    public interface IRelatedLink : ILink, IEquatable<IRelatedLink>
    {
        string LinkSubType { get; }

        IWorkItemLinkTypeEnd LinkTypeEnd { get; }

        int RelatedWorkItemId { get; }
    }
}