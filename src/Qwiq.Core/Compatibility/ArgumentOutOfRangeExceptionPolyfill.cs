// Polyfill for ArgumentOutOfRangeException throw helpers for .NET Framework and .NET Standard 2.0
// Based on: https://github.com/SimonCropp/Polyfill
// We cannot use the Polyfill NuGet package directly due to conflicts with VSS Client polyfills.

#pragma warning disable

using System;
using System.Runtime.CompilerServices;

static partial class Polyfill
{
    extension(ArgumentOutOfRangeException)
    {
#if !NET8_0_OR_GREATER
        // Link: https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception.throwifzero
        public static void ThrowIfZero(
            int value,
            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be zero.");
            }
        }

        // Link: https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception.throwifequal
        public static void ThrowIfEqual<T>(
            T value,
            T other,
            [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IEquatable<T>?
        {
            if (EqualityComparer<T>.Default.Equals(value, other))
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Value cannot be equal to {other}.");
            }
        }

        // Link: https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception.throwifnegative
        public static void ThrowIfNegative(
            int value,
            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");
            }
        }

        // Link: https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception.throwifnegativeorzero
        public static void ThrowIfNegativeOrZero(
            int value,
            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative or zero.");
            }
        }
#endif
    }
}
