using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Qwiq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Should
{
    /// <summary>
    /// Compatibility shim that bridges the old Should library API to Shouldly.
    /// This allows existing tests to compile without modification.
    /// </summary>
    [DebuggerStepThrough]
    public static class ShouldExtensions
    {
        public static void ShouldContainOnly<T>(this IEnumerable<T> collection, IEnumerable<T> expected)
        {
            ShouldContainOnly(collection, expected, GenericComparer<T>.Default);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> collection, params T[] expected)
        {
            ShouldContainOnly(collection, expected, GenericComparer<T>.Default);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> collection, IEnumerable<T> expected, IEqualityComparer<T> comparer)
        {
            var source = new List<T>(collection);
            var noContain = new List<T>();

            foreach (var item in expected)
            {
                if (!source.Contains(item, comparer)) noContain.Add(item);
                else source.Remove(item);
            }

            if (noContain.Count > 0 || source.Count > 0)
            {
                var message = $"Should contain only: {expected.EachToUsefulString()} \r\nentire list: {collection.EachToUsefulString()}";

                if (noContain.Count > 0) message += "\ndoes not contain: " + noContain.EachToUsefulString();

                if (source.Count > 0) message += "\ndoes contain but shouldn't: " + source.EachToUsefulString();

                throw new AssertFailedException(message);
            }
        }

        // Bridge methods from Should library API to Shouldly

        /// <summary>
        /// Asserts that the value equals the expected value.
        /// </summary>
        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Shouldly.ShouldBeTestExtensions.ShouldBe(actual, expected);
        }

        /// <summary>
        /// Asserts that the value equals the expected value with a custom message.
        /// </summary>
        public static void ShouldEqual<T>(this T actual, T expected, string customMessage)
        {
            Shouldly.ShouldBeTestExtensions.ShouldBe(actual, expected, customMessage);
        }

        /// <summary>
        /// Asserts that the value equals the expected value using a custom comparer.
        /// </summary>
        public static void ShouldEqual<T>(this T actual, T expected, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(actual, expected))
            {
                throw new AssertFailedException($"Expected {expected} but was {actual}");
            }
        }

        /// <summary>
        /// Asserts that the value equals the expected value using a custom comparer.
        /// Since Shouldly doesn't natively support IEqualityComparer, this shim provides the functionality.
        /// </summary>
        public static void ShouldBe<T>(this T actual, T expected, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(actual, expected))
            {
                throw new AssertFailedException($"Expected {expected} but was {actual} (using custom comparer)");
            }
        }

        /// <summary>
        /// Asserts that the value is null.
        /// </summary>
        public static void ShouldBeNull<T>(this T? actual) where T : class
        {
            Shouldly.ShouldBeNullExtensions.ShouldBeNull(actual);
        }

        /// <summary>
        /// Asserts that the value is not null.
        /// </summary>
        public static void ShouldNotBeNull<T>(this T? actual) where T : class
        {
            Shouldly.ShouldBeNullExtensions.ShouldNotBeNull(actual);
        }

        /// <summary>
        /// Asserts that the value is not null with a custom message.
        /// </summary>
        public static void ShouldNotBeNull<T>(this T? actual, string customMessage) where T : class
        {
            Shouldly.ShouldBeNullExtensions.ShouldNotBeNull(actual, customMessage);
        }

        /// <summary>
        /// Asserts that the boolean is true.
        /// </summary>
        public static void ShouldBeTrue(this bool actual)
        {
            Shouldly.ShouldBeBooleanExtensions.ShouldBeTrue(actual);
        }

        /// <summary>
        /// Asserts that the boolean is false.
        /// </summary>
        public static void ShouldBeFalse(this bool actual)
        {
            Shouldly.ShouldBeBooleanExtensions.ShouldBeFalse(actual);
        }

        /// <summary>
        /// Asserts that the collection is empty.
        /// </summary>
        public static void ShouldBeEmpty<T>(this IEnumerable<T> actual)
        {
            Shouldly.ShouldBeEnumerableTestExtensions.ShouldBeEmpty(actual);
        }

        /// <summary>
        /// Asserts that the value is of the specified type.
        /// </summary>
        public static void ShouldBeType<T>(this object? actual)
        {
            Shouldly.ShouldBeNullExtensions.ShouldNotBeNull(actual);
            Shouldly.ShouldBeTestExtensions.ShouldBeOfType<T>(actual);
        }

        /// <summary>
        /// Asserts that the value is of the specified type (non-generic overload).
        /// </summary>
        public static void ShouldBeType(this object? actual, Type expectedType)
        {
            Shouldly.ShouldBeNullExtensions.ShouldNotBeNull(actual);
            Shouldly.ShouldBeTestExtensions.ShouldBeOfType(actual, expectedType);
        }

        /// <summary>
        /// Asserts that the string contains the expected substring.
        /// </summary>
        public static void ShouldContain(this string actual, string expected)
        {
            Shouldly.ShouldBeStringTestExtensions.ShouldContain(actual, expected);
        }

        /// <summary>
        /// Asserts that the collection contains the expected item.
        /// </summary>
        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            Shouldly.ShouldBeEnumerableTestExtensions.ShouldContain(actual, expected);
        }

        /// <summary>
        /// Asserts that the value is greater than the specified value.
        /// </summary>
        public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable<T>
        {
            Shouldly.ShouldBeTestExtensions.ShouldBeGreaterThan(actual, expected);
        }

        /// <summary>
        /// Asserts that the value is less than the specified value.
        /// </summary>
        public static void ShouldBeLessThan<T>(this T actual, T expected) where T : IComparable<T>
        {
            Shouldly.ShouldBeTestExtensions.ShouldBeLessThan(actual, expected);
        }
    }
}