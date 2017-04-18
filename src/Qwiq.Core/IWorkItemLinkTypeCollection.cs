using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkTypeCollection : IReadOnlyList<IWorkItemLinkType>,
                                                   IEquatable<IWorkItemLinkTypeCollection>
    {
        IWorkItemLinkTypeEndCollection LinkTypeEnds { get; }
    }
}