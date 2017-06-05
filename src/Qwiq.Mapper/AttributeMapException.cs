using System;

namespace Microsoft.Qwiq.Mapper
{
    public class AttributeMapException : Exception
    {
        public AttributeMapException()
        {
        }

        public AttributeMapException(string message)
            :base(message)
        {
        }

        public AttributeMapException(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
}
