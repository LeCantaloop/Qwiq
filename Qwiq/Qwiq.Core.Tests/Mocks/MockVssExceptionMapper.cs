using System;
using Microsoft.IE.Qwiq.Exceptions;

namespace Microsoft.IE.Qwiq.Core.Tests.Mocks
{
    internal class MockVssExceptionMapper<T> : VssExceptionMapper where T : Exception, new()
    {
        public MockVssExceptionMapper(int[] handledErrorCodes) : base(handledErrorCodes, (s, exception) => new T())
        {
        }
    }
}