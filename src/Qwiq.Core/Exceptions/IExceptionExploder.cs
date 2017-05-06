using System;
using System.Collections.ObjectModel;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Exceptions
{
    public interface IExceptionExploder
    {
        [CanBeNull]
        ReadOnlyCollection<Exception> Explode([CanBeNull] Exception exception);
    }
}

