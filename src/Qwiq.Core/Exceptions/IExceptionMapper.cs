using System;

namespace Microsoft.Qwiq.Exceptions
{
    public interface IExceptionMapper
    {
        Exception Map(Exception ex);
    }
}

