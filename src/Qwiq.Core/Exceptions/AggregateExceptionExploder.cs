using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Qwiq.Exceptions
{
    internal class AggregateExceptionExploder : IExceptionExploder
    {
        private static readonly ReadOnlyCollection<Exception> Empty = new ReadOnlyCollection<Exception>(new List<Exception>());

        public ReadOnlyCollection<Exception> Explode(Exception exception)
        {
            if (exception is AggregateException ae)
            {
                var ae1 = ae.Flatten();
                return ae1.InnerExceptions;
            }

            return Empty;
        }
    }
}