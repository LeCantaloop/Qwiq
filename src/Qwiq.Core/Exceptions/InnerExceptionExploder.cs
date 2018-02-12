using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Qwiq.Exceptions
{
    [DebuggerStepThrough]
    internal class InnerExceptionExploder : IExceptionExploder
    {
        private static readonly ReadOnlyCollection<Exception> Empty = new ReadOnlyCollection<Exception>(new List<Exception>());

        public ReadOnlyCollection<Exception> Explode(Exception exception)
        {
            if (exception?.InnerException == null) return Empty;

            // REVIEW: Reference type object allocation is extraneous
            var l = new List<Exception>(1)
            {
                exception.InnerException
            };
            return l.AsReadOnly();
        }
    }
}