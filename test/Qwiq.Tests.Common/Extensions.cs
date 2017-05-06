using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Qwiq
{
    public static class Extensions
    {
        public static string EachToUsefulString<T>(this IEnumerable<T> enumerable, int limit = 10)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.Append(string.Join(",\n", enumerable.Select(x => ToUsefulString(x).Tab()).Take(limit).ToArray()));
            if (enumerable.Count() > limit)
            {
                if (enumerable.Count() > (limit+1))
                {
                    sb.AppendLine($",\n  ...({enumerable.Count() - limit} more elements)");
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

        public static string ToUsefulString(this object obj)
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
                var enumerable = ((IEnumerable)obj).Cast<object>();

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

            var split = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            sb.Append("  " + split[0]);
            foreach (var part in split.Skip(1))
            {
                sb.AppendLine();
                sb.Append("  " + part);
            }

            return sb.ToString();
        }
    }
}