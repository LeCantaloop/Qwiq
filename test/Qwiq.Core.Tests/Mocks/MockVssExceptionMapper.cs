using System;

using Qwiq.Exceptions;

namespace Qwiq.Mocks
{
    internal class MockVssExceptionMapper<T> : VssExceptionMapper where T : Exception, new()
    {
        public MockVssExceptionMapper(int[] handledErrorCodes) : base(handledErrorCodes, (s, exception) => new T())
        {
        }
    }
}
