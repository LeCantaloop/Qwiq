using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Microsoft.IE.Qwiq.Exceptions
{
    public class ExceptionHandlingDynamicProxy<T> : RealProxy
    {
        private readonly T _decorated;
        private readonly IExceptionMapper _exceptionMapper;

        public ExceptionHandlingDynamicProxy(T decorated, IExceptionMapper exceptionMapper) : base(typeof(T))
        {
            _decorated = decorated;
            _exceptionMapper = exceptionMapper;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;

            try
            {
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                return new ReturnMessage(_exceptionMapper.Map(e.InnerException), methodCall);
            }
        }
    }
}
