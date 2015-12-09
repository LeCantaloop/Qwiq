using System;

namespace Microsoft.IE.Qwiq.Core.Tests.Mocks
{
    public class ExceptionThrower : IExceptionThrower
    {
        private readonly string _argumentName;

        public ExceptionThrower(string argumentName)
        {
            _argumentName = argumentName;
        }

        public void ThrowException()
        {
            throw new ArgumentException(null, _argumentName); ;
        }
    }

    public interface IExceptionThrower
    {
        void ThrowException();
    }
}
