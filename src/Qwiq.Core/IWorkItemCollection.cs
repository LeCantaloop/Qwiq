using System;

namespace Qwiq
{
    public interface IWorkItemCollection : IReadOnlyObjectWithIdCollection<IWorkItem, int>, IEquatable<IWorkItemCollection>
    {

    }
}