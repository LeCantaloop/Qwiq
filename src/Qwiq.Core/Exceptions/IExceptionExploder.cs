using System;
using System.Collections.ObjectModel;

using JetBrains.Annotations;

namespace Qwiq.Exceptions
{
    public interface IExceptionExploder
    {
        [CanBeNull]
        ReadOnlyCollection<Exception> Explode([CanBeNull] Exception exception);
    }
}

