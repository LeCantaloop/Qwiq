using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkTypeCollection : IReadOnlyCollection<IWorkItemLinkType>,
                                                   IEquatable<IWorkItemLinkTypeCollection>
    {
        IWorkItemLinkTypeEndCollection LinkTypeEnds { get; }
    }
}