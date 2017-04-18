using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Qwiq.Exceptions
{
    [DebuggerStepThrough]
    internal class InnerExceptionExploder : IExceptionExploder
    {
        public IEnumerable<Exception> Explode(Exception exception)
        {
            if (exception.InnerException != null) return new[] { exception.InnerException};
            return Enumerable.Empty<Exception>();
        }
    }
}

