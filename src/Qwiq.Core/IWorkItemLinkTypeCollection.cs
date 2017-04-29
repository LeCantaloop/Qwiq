using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkTypeCollection : IReadOnlyObjectList<IWorkItemLinkType>,
                                                   IEquatable<IWorkItemLinkTypeCollection>
    {
        IWorkItemLinkTypeEndCollection LinkTypeEnds { get; }
    }
}