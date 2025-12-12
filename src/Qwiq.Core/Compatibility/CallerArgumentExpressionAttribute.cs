// Polyfill for CallerArgumentExpressionAttribute for .NET Framework and .NET Standard 2.0
// This attribute is used by ArgumentOutOfRangeExceptionPolyfill.cs

#if NETFRAMEWORK || NETSTANDARD2_0

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows capturing the expressions passed to a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the targeted parameter.</param>
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the target parameter name of the CallerArgumentExpression.
        /// </summary>
        public string ParameterName { get; }
    }
}

#endif
