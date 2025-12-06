using System;
using System.Diagnostics;
#if NETFRAMEWORK || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

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

#if NETFRAMEWORK || NETSTANDARD2_0
        protected TransientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}