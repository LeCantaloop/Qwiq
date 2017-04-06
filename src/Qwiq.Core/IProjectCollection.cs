using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IProjectCollection : IEnumerable<IProject>
    {
        IProject this[string projectName] { get; }

        IProject this[Guid id] { get; }
    }
}