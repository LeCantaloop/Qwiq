using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;

using Castle.DynamicProxy;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Exceptions
{
    [DebuggerStepThrough]
    public class ExceptionHandlingDynamicProxy : IInterceptor
    {
        [NotNull]
        private readonly IExceptionMapper _exceptionMapper;

        public ExceptionHandlingDynamicProxy([NotNull] IExceptionMapper exceptionMapper)
        {
            Contract.Requires(exceptionMapper != null);

            _exceptionMapper = exceptionMapper;
        }

        public void Intercept([NotNull] IInvocation invocation)
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_exceptionMapper != null);
        }
    }
}