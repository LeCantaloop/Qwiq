using System;
using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Core.Tests.Mocks
{
    internal class MockVssExceptionMapper<T> : VssExceptionMapper where T : Exception, new()
    {
        public MockVssExceptionMapper(int[] handledErrorCodes) : base(handledErrorCodes, (s, exception) => new T())
        {
        }
    }
}
