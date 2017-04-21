using System;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq.Core.Tests.Mocks
{
    [Serializable]
    public class MockException : Exception
    {
        public MockException()
            :base()
        {
        }

        protected MockException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

