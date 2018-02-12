using System;

using JetBrains.Annotations;

namespace Qwiq.Exceptions
{
    public interface IExceptionMapper
    {
        [CanBeNull]
        Exception Map(Exception ex);
    }
}

