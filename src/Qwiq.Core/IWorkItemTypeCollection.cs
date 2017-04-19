using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IReadOnlyCollection<IWorkItemType>, IEquatable<IWorkItemTypeCollection>
    {
    }
}