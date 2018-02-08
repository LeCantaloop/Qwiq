using System;
using System.Runtime.Serialization;

namespace Qwiq.UnitTests.Mocks
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

