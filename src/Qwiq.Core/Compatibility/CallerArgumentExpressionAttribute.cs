#if NETFRAMEWORK || NETSTANDARD2_0

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [global::System.AttributeUsage(
        global::System.AttributeTargets.Parameter,
        AllowMultiple = false,
        Inherited = false)]
    sealed class CallerArgumentExpressionAttribute :
        global::System.Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName) =>
            ParameterName = parameterName;

        public string ParameterName { get; }
    }
}

#endif
