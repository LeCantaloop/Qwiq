using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Qwiq.Identity
{
    [Serializable]
    public class MultipleIdentitiesFoundException : ApplicationException
    {
        public MultipleIdentitiesFoundException(string displayName, IEnumerable<string> matches)
            : this($"Multiple identities found matching '{displayName}'. Please specify one of the following identities:\r\n- {string.Join("\r\n- ", matches)}")
        {
        }

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