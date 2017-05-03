using Castle.DynamicProxy;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Microsoft.Qwiq.Exceptions
{
    public static class ExceptionHandlingDynamicProxyFactory
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        private static readonly ProxyGenerationOptions Options =
                new ProxyGenerationOptions { BaseTypeForInterfaceProxy = typeof(ProxyBase) };

        [JetBrains.Annotations.Pure]
        [NotNull]
        public static T Create<T>([NotNull] T instance)
            where T : class
        {
            Contract.Requires(instance != null);
            Contract.Ensures(Contract.Result<T>() != null);

            return Create(
                          instance,
                          new IExceptionExploder[] { new AggregateExceptionExploder(), new InnerExceptionExploder() },
                          new IExceptionMapper[] { new InvalidOperationExceptionMapper(), new TransientExceptionMapper() });
        }

        [NotNull]
        [JetBrains.Annotations.Pure]
        internal static T Create<T>(
            [NotNull] T instance,
            [NotNull] IEnumerable<IExceptionExploder> exploders,
            [NotNull] IEnumerable<IExceptionMapper> mappers)
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