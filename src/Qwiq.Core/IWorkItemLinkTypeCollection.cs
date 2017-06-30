using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkTypeCollection : IReadOnlyObjectWithNameCollection<IWorkItemLinkType>,
                                                   IEquatable<IWorkItemLinkTypeCollection>
    {
        IWorkItemLinkTypeEndCollection LinkTypeEnds { get; }
    }
}