using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IReadOnlyList<IWorkItemType>, IEquatable<IWorkItemTypeCollection>
    {
    }
}