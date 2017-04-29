using System;

namespace Microsoft.Qwiq
{
    public interface IProjectCollection : IReadOnlyObjectList<IProject>
    {
        IProject this[Guid id] { get; }
    }
}