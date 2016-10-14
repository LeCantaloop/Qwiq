using System;

namespace Microsoft.Qwiq.Exceptions
{
    public class TransientException : Exception
    {
        public TransientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

