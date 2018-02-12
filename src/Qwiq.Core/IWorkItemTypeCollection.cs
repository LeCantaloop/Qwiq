using System;

namespace Qwiq
{
    public interface IWorkItemTypeCollection : IReadOnlyObjectWithNameCollection<IWorkItemType>, IEquatable<IWorkItemTypeCollection>
    {
    }
}