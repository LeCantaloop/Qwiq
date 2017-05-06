using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        [NotNull]
        private static readonly string[] Split = { "\r\n", "\n" };

        internal static string EachToUsefulString<T>(this IEnumerable<T> enumerable, int limit = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append(string.Join(",\n", enumerable.Select(x => ToUsefulString(x).Tab()).Take(limit).ToArray()));
            if (enumerable.Count() > limit)
                if (enumerable.Count() > limit + 1) sb.AppendLine($",\n  ...({enumerable.Count() - limit} more elements)");
                else sb.AppendLine(",\n" + enumerable.Last().ToUsefulString().Tab());
            else sb.AppendLine();

            sb.AppendLine("}");

            return sb.ToString();
        }

        internal static string ToUsefulString([CanBeNull] this object obj)
        {
            string str;
            if (obj == null) return "[null]";

            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;
                return "\"" + str.Replace("\n", "\\n").Replace("\r", "\\r") + "\"";
            }

            if (obj.GetType().IsValueType) return "[" + obj + "]";

            if (obj is IEnumerable enumerable)
            {
                var e = enumerable.Cast<object>();

                return enumerable.GetType() + ":\n" + e.EachToUsefulString();
            }

            str = obj.ToString();

            if (string.IsNullOrEmpty(str)) return $"{obj.GetType()}:[]";

            str = str.Trim();

            if (str.Contains("\n")) return string.Format("{1}:\r\n[\r\n{0}\r\n]", str.Tab(), obj.GetType());

            return obj.GetType().ToString() == str ? obj.GetType().ToString() : $"{obj.GetType()}:[{str}]";
        }

        [NotNull]
        private static string Tab([CanBeNull] this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            var split = str.Split(Split, StringSplitOptions.None);
            var sb = new StringBuilder();

            sb.Append("  ");
            sb.Append(split[0]);
            foreach (var part in split.Skip(1))
            {
                sb.AppendLine();
                sb.Append("  ");
                sb.Append(part);
            }

            return sb.ToString();
        }
    }
}