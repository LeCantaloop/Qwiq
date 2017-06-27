using System;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq.Identity
{
    [Serializable]
    public class MultipleIdentitiesFoundException : ApplicationException
    {
        public MultipleIdentitiesFoundException(string message)
            : base(message)
        {
        }

        public MultipleIdentitiesFoundException() : base()
        {
        }

        public MultipleIdentitiesFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipleIdentitiesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}