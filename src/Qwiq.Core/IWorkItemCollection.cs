using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemCollection : IReadOnlyObjectWithIdCollection<IWorkItem, int>, IEquatable<IWorkItemCollection>
    {

    }
}