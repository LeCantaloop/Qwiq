using System;

using Qwiq.Exceptions;

namespace Qwiq.UnitTests.Mocks
{
    public class MockArgumentExceptionMapper : IExceptionMapper
    {
        public int ExecutionCount { get; set; }

        public const string MockParamName = "MockParam";

        public Exception Map(Exception ex)
        {
            ExecutionCount++;
            var argumentException = ex as ArgumentException;
            return (argumentException != null && argumentException.ParamName == MockParamName) ? new MockException() : null;
        }
    }
}

