using System;

namespace Qwiq
{
    public interface IWorkItemLinkTypeCollection : IReadOnlyObjectWithNameCollection<IWorkItemLinkType>,
                                                   IEquatable<IWorkItemLinkTypeCollection>
    {
        IWorkItemLinkTypeEndCollection LinkTypeEnds { get; }
    }
}