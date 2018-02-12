using System;

namespace Qwiq
{
    public interface IProjectCollection : IReadOnlyObjectWithNameCollection<IProject>
    {
        IProject this[Guid id] { get; }
    }
}