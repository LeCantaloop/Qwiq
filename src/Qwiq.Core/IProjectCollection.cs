using System;

namespace Microsoft.Qwiq
{
    public interface IProjectCollection : IReadOnlyList<IProject>
    {
        IProject this[Guid id] { get; }
    }
}