using System.Collections.Generic;
using JetBrains.Annotations;

namespace System
{
    public static class TypeExtensions
    {
        // Default values of value types return by the default ctor
        // See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/default-values-table
        private static readonly Dictionary<Type, object> DefaultValuesForTypes = new Dictionary<Type, object>
        {
            [typeof(bool)] = false,
            [typeof(bool?)] = null,
            [typeof(byte)] = (byte)0,
            [typeof(byte?)] = null,
            [typeof(char)] = '\0',
            [typeof(decimal)] = 0.0M,
            [typeof(decimal?)] = null,
            [typeof(double)] = 0.0D,
            [typeof(double?)] = null,
            [typeof(float)] = 0.0F,
            [typeof(float?)] = null,
            [typeof(int)] = 0,
            [typeof(int?)] = null,
            [typeof(long)] = 0L,
            [typeof(long?)] = null,
            [typeof(sbyte)] = (sbyte)0,
            [typeof(sbyte?)] = null,
            [typeof(short)] = (short)0,
            [typeof(short?)] = null,
            [typeof(uint)] = (uint)0,
            [typeof(uint?)] = null,
            [typeof(ulong)] = (ulong)0,
            [typeof(ulong?)] = null,
            [typeof(ushort)] = (ushort)0,
            [typeof(ushort?)] = null,
            [typeof(DateTime)] = DateTime.MinValue,
            [typeof(DateTime?)] = null,
            [typeof(DateTimeOffset)] = DateTimeOffset.MinValue,
            [typeof(DateTimeOffset?)] = null,
        };

        public static bool CanAcceptNull([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (IsGenericNullable(type))
            {
                return true;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                case TypeCode.String:
                    return true;
            }

            return false;
        }

        [CanBeNull]
        public static object GetDefaultValueOfType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (DefaultValuesForTypes.TryGetValue(type, out object retval))
            {
                return retval;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool IsGenericNullable([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}