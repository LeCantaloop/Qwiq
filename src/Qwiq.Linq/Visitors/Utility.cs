using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Qwiq.Linq.Visitors
{
    public static class Utility
    {
        public static string ToConcatenatedString<T>(this IEnumerable<T> source, Func<T, string> selector, string separator)
        {
            var b = new StringBuilder();
            var needSeparator = false;

            foreach (var item in source)
            {
                if (needSeparator) b.Append(separator);

                b.Append(selector(item));
                needSeparator = true;
            }

            return b.ToString();
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        {
            return new LinkedList<T>(source);
        }
    }
}