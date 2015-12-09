using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Exceptions
{
    public interface IExceptionExploder
    {
        IEnumerable<Exception> Explode(Exception exception);
    }
}
