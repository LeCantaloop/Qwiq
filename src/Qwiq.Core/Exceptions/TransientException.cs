using System;
using System.Diagnostics;

namespace Microsoft.Qwiq.Exceptions
{
    [DebuggerStepThrough]
    public class TransientException : Exception
    {
        public TransientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

