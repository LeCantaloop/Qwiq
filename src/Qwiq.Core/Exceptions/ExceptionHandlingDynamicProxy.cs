using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;

namespace Qwiq.Exceptions
{
    [DebuggerStepThrough]
    public class ExceptionHandlingDynamicProxy : IInterceptor
    {
        private readonly IExceptionMapper _exceptionMapper;

        public ExceptionHandlingDynamicProxy(IExceptionMapper exceptionMapper)
        {
            Contract.Requires(exceptionMapper != null);

            _exceptionMapper = exceptionMapper ?? throw new ArgumentNullException(nameof(exceptionMapper));
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
                ExceptionDispatchInfo.Capture(_exceptionMapper.Map(e) ?? e).Throw();
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_exceptionMapper != null);
        }
    }
}