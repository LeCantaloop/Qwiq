using System;

namespace Microsoft.Qwiq
{
    public interface IProjectCollection : IReadOnlyCollection<IProject>
    {
        IProject this[Guid id] { get; }
    }
}