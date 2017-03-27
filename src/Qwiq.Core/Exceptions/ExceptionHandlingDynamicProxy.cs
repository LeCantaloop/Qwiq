using System;
using System.Diagnostics;

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
                throw _exceptionMapper.Map(e);
            }
        }
    }
}

