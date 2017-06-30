using System;

namespace Microsoft.Qwiq
{
    public interface IProjectCollection : IReadOnlyObjectWithNameCollection<IProject>
    {
        IProject this[Guid id] { get; }
    }
}