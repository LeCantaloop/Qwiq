using System;

namespace Microsoft.Qwiq
{
    public interface IRelatedLink : ILink, IEquatable<IRelatedLink>
    {
        [Obsolete("This property is deprecated and will be removed in a future release. Use LinkTypeEnd.Name instead.")]
        string LinkSubType { get; }

        IWorkItemLinkTypeEnd LinkTypeEnd { get; }

        int RelatedWorkItemId { get; }
    }
}