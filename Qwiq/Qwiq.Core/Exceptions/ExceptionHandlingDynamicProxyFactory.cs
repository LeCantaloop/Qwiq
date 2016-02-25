namespace Microsoft.IE.Qwiq.Exceptions
{
    public static class ExceptionHandlingDynamicProxyFactory
    {
        public static T Create<T>(T instance) where T : class
        {
            return new ExceptionHandlingDynamicProxy<T>(
                instance,
                new ExceptionMapper(
                    new IExceptionExploder[] {new AggregateExceptionExploder(), new InnerExceptionExploder()},
                    new IExceptionMapper[] {new InvalidOperationExceptionMapper(), new TransientExceptionMapper()})).GetTransparentProxy() as T;
        }
    }
}
