using System;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#endif

namespace Qwiq.UnitTests.Mocks
{
    [Serializable]
    public class MockException : Exception
    {
        public MockException()
            : base()
        {
        }

#if NETFRAMEWORK
        protected MockException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}

