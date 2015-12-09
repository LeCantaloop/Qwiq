using System;

namespace Microsoft.IE.Qwiq.Exceptions
{
    public interface IExceptionMapper
    {
        Exception Map(Exception ex);
    }
}
