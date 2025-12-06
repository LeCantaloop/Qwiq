using System;
#if NETFRAMEWORK || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

namespace Qwiq
{
    [Serializable]
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string message) : base(message)
        {
        }

        public AccessDeniedException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NETFRAMEWORK || NETSTANDARD2_0
        protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
