using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemCollection : IReadOnlyObjectWithIdList<IWorkItem, int>, IEquatable<IWorkItemCollection>
    {

    }
}