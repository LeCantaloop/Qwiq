using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Qwiq.Exceptions
{
    [Serializable]
    [DebuggerStepThrough]
    public class TransientException : Exception
    {
        public TransientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TransientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}