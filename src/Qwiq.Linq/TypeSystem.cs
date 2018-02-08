using System;
using System.Collections.Generic;

namespace Qwiq.Linq
{
    // Boiler plate
    // Developed from http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx
    internal static class TypeSystem
    {
        /// <summary>
        /// Gets the type of of an object. If that object is a collection
        /// (i.e. can be an IEnumerable), returns the *inner* type.
        /// </summary>
        /// <param name="type"></param>
        /// <example>
        /// For example. Calling TypeSystem.GetElementType(typeof(int))
        /// will return 'int'.
        ///
        /// Calling TypeSystem.GetElementType(typeof(string[])) will
        /// return 'String'.
        /// </example>
        /// <returns></returns>
        public static Type GetElementType(Type type)
        {
            var iEnumerable = FindIEnumerable(type);
            return iEnumerable == null ? type : iEnumerable.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type type)
        {
            if (type == null || type == typeof(string)) { return null; }
            if (type.IsArray) { return typeof(IEnumerable<>).MakeGenericType(type.GetElementType()); }

            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    var iEnumerable = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (iEnumerable.IsAssignableFrom(type))
                    {
                        return iEnumerable;
                    }
                }
            }

            var ifaces = type.GetInterfaces();

            if (ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var iEnumerable = FindIEnumerable(iface);
                    if (iEnumerable != null) { return iEnumerable; }
                }
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return FindIEnumerable(type.BaseType);
            }

            return null;
        }
    }
}

