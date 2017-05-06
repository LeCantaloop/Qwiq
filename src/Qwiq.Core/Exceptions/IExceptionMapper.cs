using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Exceptions
{
    public interface IExceptionMapper
    {
        [CanBeNull]
        Exception Map(Exception ex);
    }
}

