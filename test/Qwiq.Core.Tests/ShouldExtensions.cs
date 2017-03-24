using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldContainOnly<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            ShouldContainOnly(list, items, GenericComparer<T>.Default);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> list, params T[] items)
        {
            ShouldContainOnly(list, items, GenericComparer<T>.Default);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> list, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            var source = new List<T>(list);
            var noContain = new List<T>();

            foreach (T item in items)
            {
                if (!source.Contains(item, comparer))
                {
                    noContain.Add(item);
                }
                else
                {
                    // this only removes the first occurrence, so if the number of occurrences doesn't match, we'll still get a
                    // valid mismatch between the lists
                    source.Remove(item);
                }
            }

            if (noContain.Any() || source.Any())
            {
                string message =
                    $"Should contain only: {items.EachToUsefulString()} \r\nentire list: {list.EachToUsefulString()}";

                if (noContain.Any())
                {
                    message += "\ndoes not contain: " + noContain.EachToUsefulString();
                }

                if (source.Any())
                {
                    message += "\ndoes contain but shouldn't: " + source.EachToUsefulString();
                }

                throw new AssertFailedException(message);
            }
        }

        private static string EachToUsefulString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append(string.Join(",\n", enumerable.Select(x => x.ToUsefulString().Tab()).Take(10).ToArray()));
            if (enumerable.Count() > 20)
            {
                if (enumerable.Count() > 21)
                {
                    sb.AppendLine($",\n  ...({enumerable.Count() - 20} more elements)");
                }
                else
                {
                    sb.AppendLine(",\n" + enumerable.Last().ToUsefulString().Tab());
                }
            }
            else
            {
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string ToUsefulString(this object obj)
        {
            string str;
            if (obj == null)
            {
                return "[null]";
            }

            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;
                return "\"" + str.Replace("\n", "\\n").Replace("\r", "\\r") + "\"";
            }

            if (obj.GetType().IsValueType)
            {
                return "[" + obj + "]";
            }

            if (obj is IEnumerable)
            {
                IEnumerable<object> enumerable = ((IEnumerable)obj).Cast<object>();

                return obj.GetType() + ":\n" + enumerable.EachToUsefulString();
            }

            str = obj.ToString();

            if (string.IsNullOrEmpty(str))
            {
                return $"{obj.GetType()}:[]";
            }

            str = str.Trim();

            if (str.Contains("\n"))
            {
                return string.Format("{1}:\r\n[\r\n{0}\r\n]", str.Tab(), obj.GetType());
            }

            if (obj.GetType().ToString() == str)
            {
                return obj.GetType().ToString();
            }

            return $"{obj.GetType()}:[{str}]";
        }

        private static string Tab(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            string[] split = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            sb.Append("  " + split[0]);
            foreach (string part in split.Skip(1))
            {
                sb.AppendLine();
                sb.Append("  " + part);
            }

            return sb.ToString();
        }
    }
}
