using Castle.DynamicProxy;

using System.Diagnostics.Contracts;

namespace Qwiq.Exceptions
{
    internal static class ExceptionHandlingDynamicProxyFactory
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private static readonly ProxyGenerationOptions Options =
                new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(ProxyBase) };
        private static readonly IExceptionExploder[] ExceptionExploders = { new AggregateExceptionExploder(), new InnerExceptionExploder() };
        private static readonly IExceptionMapper[] ExceptionMappers = { new InvalidOperationExceptionMapper(), new TransientExceptionMapper() };
        private static readonly ExceptionHandlingDynamicProxy Proxy = new ExceptionHandlingDynamicProxy(new ExceptionMapper(ExceptionExploders, ExceptionMappers));

        internal static T Create<T>(T instance)
            where T : class
        {
            Contract.Requires(instance != null);
            Contract.Ensures(Contract.Result<T>() != null);

            return (T)Generator.CreateInterfaceProxyWithTarget(typeof(T), instance, Options, Proxy);
        }
        internal static T Create<T>(
            T instance,
            IExceptionExploder[]? exploders,
            IExceptionMapper[]? mappers)
            where T : class
        {
            Contract.Requires(instance != null);

            var proxy = new ExceptionHandlingDynamicProxy(new ExceptionMapper(exploders ?? ExceptionExploders, mappers ?? ExceptionMappers));

            return (T)Generator.CreateInterfaceProxyWithTarget(typeof(T), instance, Options, proxy);
        }
    }
}