using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemCollection : IReadOnlyCollectionWithId<IWorkItem, int>, IEquatable<IWorkItemCollection>
    {

    }
}