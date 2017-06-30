using Castle.DynamicProxy;
using JetBrains.Annotations;

using System.Diagnostics.Contracts;

namespace Microsoft.Qwiq.Exceptions
{
    internal static class ExceptionHandlingDynamicProxyFactory
    {
        [NotNull]
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        [NotNull]
        private static readonly ProxyGenerationOptions Options =
                new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(ProxyBase) };

        [NotNull]
        private static readonly IExceptionExploder[] ExceptionExploders = { new AggregateExceptionExploder(), new InnerExceptionExploder() };

        [NotNull]
        private static readonly IExceptionMapper[] ExceptionMappers = { new InvalidOperationExceptionMapper(), new TransientExceptionMapper() };

        [NotNull]
        private static readonly ExceptionHandlingDynamicProxy Proxy = new ExceptionHandlingDynamicProxy(new ExceptionMapper(ExceptionExploders, ExceptionMappers));

        [JetBrains.Annotations.Pure]
        [NotNull]
        internal static T Create<T>([NotNull] T instance)
            where T : class
        {
            Contract.Requires(instance != null);
            Contract.Ensures(Contract.Result<T>() != null);

            return (T)Generator.CreateInterfaceProxyWithTarget(typeof(T), instance, Options, Proxy);
        }

        [NotNull]
        [JetBrains.Annotations.Pure]
        internal static T Create<T>(
            [NotNull] T instance,
            [NotNull] IExceptionExploder[] exploders,
            [NotNull] IExceptionMapper[] mappers)
            where T : class
        {
            Contract.Requires(instance != null);
            Contract.Requires(exploders != null);
            Contract.Requires(mappers != null);

            var proxy = new ExceptionHandlingDynamicProxy(new ExceptionMapper(exploders, mappers));

            return (T)Generator.CreateInterfaceProxyWithTarget(typeof(T), instance, Options, proxy);
        }
    }
}