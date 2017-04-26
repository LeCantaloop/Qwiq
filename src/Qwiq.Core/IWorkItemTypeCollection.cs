using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IReadOnlyObjectList<IWorkItemType>, IEquatable<IWorkItemTypeCollection>
    {
    }
}