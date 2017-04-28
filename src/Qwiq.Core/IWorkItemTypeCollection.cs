using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IReadOnlyObjectWithNameCollection<IWorkItemType>, IEquatable<IWorkItemTypeCollection>
    {
    }
}