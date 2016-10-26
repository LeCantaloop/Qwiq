using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Exceptions
{
    public interface IExceptionExploder
    {
        IEnumerable<Exception> Explode(Exception exception);
    }
}

