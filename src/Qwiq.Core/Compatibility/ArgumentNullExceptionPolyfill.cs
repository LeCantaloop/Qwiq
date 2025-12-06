// Polyfill for ArgumentNullException.ThrowIfNull for .NET Framework and .NET Standard 2.0
// Based on: https://github.com/SimonCropp/Polyfill
// We cannot use the Polyfill NuGet package directly due to conflicts with VSS Client polyfills.

#pragma warning disable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

static partial class Polyfill
{
    extension(ArgumentNullException)
    {
#if !NET6_0_OR_GREATER
        // Link: https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception.throwifnull
        public static void ThrowIfNull(
            [NotNull] object? argument,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
#endif
    }
}
