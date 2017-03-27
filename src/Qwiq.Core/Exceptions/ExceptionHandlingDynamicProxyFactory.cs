using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Microsoft.Qwiq.Exceptions
{
    public static class ExceptionHandlingDynamicProxyFactory
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private static readonly ProxyGenerationOptions Options = new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(ProxyBase) };

        public static T Create<T>(T instance) where T : class
        {
            return Create(
                    instance,
                    new IExceptionExploder[]
                    {
                        new AggregateExceptionExploder(),
                        new InnerExceptionExploder()
                    },
                    new IExceptionMapper[]
                    {
                        new InvalidOperationExceptionMapper(),
                        new TransientExceptionMapper()
                    });
        }

        internal static T Create<T>(T instance, IEnumerable<IExceptionExploder> exploders, IEnumerable<IExceptionMapper> mappers) where T : class
        {
            var proxy =
                new ExceptionHandlingDynamicProxy<T>(
                    new ExceptionMapper(
                        exploders,
                        mappers));

            return (T)Generator.CreateInterfaceProxyWithTarget(typeof(T), instance, Options, proxy);
        }
    }
}

