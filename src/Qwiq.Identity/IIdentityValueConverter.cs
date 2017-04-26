namespace Microsoft.Qwiq.Identity
{
    /// <summary>
    /// Defines a method that converts the value of the implementing reference or value type to another reference or value type.
    /// </summary>
    public interface IIdentityValueConverter
    {
        /// <summary>
        /// Converts the specified <paramref name="value"/> to an <see cref="object"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An <see cref="object"/> instance whose value is equivalent to the value of <paramref name="value"/>.</returns>
        object Map(object value);
    }
}
