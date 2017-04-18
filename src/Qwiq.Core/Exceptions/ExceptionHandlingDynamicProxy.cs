using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

using Castle.DynamicProxy;

namespace Microsoft.Qwiq.Exceptions
{
    [DebuggerStepThrough]
    public class ExceptionHandlingDynamicProxy<T> : IInterceptor
    {
        private readonly IExceptionMapper _exceptionMapper;

        public ExceptionHandlingDynamicProxy(IExceptionMapper exceptionMapper)
        {
            _exceptionMapper = exceptionMapper;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                // .NET 4.5 feature: Capture an exception and re-throw it without changing the stack trace
                ExceptionDispatchInfo.Capture(_exceptionMapper.Map(e)).Throw();
            }
        }
    }
}

