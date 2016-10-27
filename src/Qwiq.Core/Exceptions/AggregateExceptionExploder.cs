using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Exceptions
{
    internal class AggregateExceptionExploder : IExceptionExploder
    {
        public IEnumerable<Exception> Explode(Exception exception)
        {
            return new [] {exception}.OfType<AggregateException>().Select(ae => ae.Flatten()).SelectMany(ae => ae.InnerExceptions);
        }
    }
}

