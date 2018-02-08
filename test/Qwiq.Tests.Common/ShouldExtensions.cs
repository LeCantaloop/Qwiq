using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Qwiq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Should
{
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

            if (noContain.Any() || source.Any())
            {
                var message = $"Should contain only: {expected.EachToUsefulString()} \r\nentire list: {collection.EachToUsefulString()}";

                if (noContain.Any()) message += "\ndoes not contain: " + noContain.EachToUsefulString();

                if (source.Any()) message += "\ndoes contain but shouldn't: " + source.EachToUsefulString();

                throw new AssertFailedException(message);
            }
        }
    }
}