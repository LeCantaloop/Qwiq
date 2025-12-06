using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Qwiq.Identity
{
    /// <summary>
    /// Defines a method that converts the value of the implementing reference or value type to another reference or value type.
    /// </summary>
    /// <typeparam name="T">The input type to convert from.</typeparam>
    /// <typeparam name="U">The output type to convert to.</typeparam>
    [SuppressMessage("Naming", "CA1715:Identifiers should have correct prefix", Justification = "U is an established type parameter name for output types in converter patterns. Changing to TU would be a breaking API change.")]
    public interface IIdentityValueConverter<T, U>
    {
        /// <summary>
        /// Converts the specified <paramref name="value"/> to an <see cref="object"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An <see cref="object"/> instance whose value is equivalent to the value of <paramref name="value"/>.</returns>
        U Map(T value);

        IReadOnlyDictionary<T, U> Map(IEnumerable<T> values);
    }
}