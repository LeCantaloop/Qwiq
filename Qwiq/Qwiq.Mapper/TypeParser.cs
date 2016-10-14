using System;
using System.ComponentModel;

namespace Microsoft.Qwiq.Mapper
{
    /// <summary>
    /// Helper class that converts objects (as they are returned from the TFS API) to usable types.
    /// </summary>
    public class TypeParser : ITypeParser
    {
        public object Parse(Type destinationType, object value, object defaultValue)
        {
            return ParseImpl(destinationType, value, new Lazy<object>(() => defaultValue));
        }

        private static object ParseImpl(Type destinationType, object value, Lazy<object> defaultValueFactory)
        {
            // If the incoming value is null, return the default value
            if (ValueRepresentsNull(value))
            {
                return defaultValueFactory.Value;
            }

            // Quit if no type conversion is actually required
            if (value.GetType() == destinationType)
            {
                return value;
            }

            if (destinationType.IsInstanceOfType(value))
            {
                return value;
            }


            object result;

            if (TryConvert(destinationType, value, out result))
            {
                return result;
            }

            if (IsGenericNullable(destinationType) && defaultValueFactory.Value == null)
            {
                return null;
            }

            if (TryConvert(destinationType, defaultValueFactory.Value, out result))
            {
                return result;
            }

            return null;
        }

        public object Parse(Type destinationType, object input)
        {
            return ParseImpl(destinationType, input, new Lazy<object>(()=> GetDefaultValueOfType(destinationType)));
        }

        public T Parse<T>(object value)
        {
            return Parse(value, default(T));
        }

        public T Parse<T>(object value, T defaultValue)
        {
            return (T)Parse(typeof(T), value, defaultValue);
        }



        private static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        private static bool IsGenericNullable(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition();
        }

        private static bool TryConvert(Type destinationType, object value, out object result)
        {
            var valueType = value.GetType();
            var typeConverter = TypeDescriptor.GetConverter(valueType);
            if (typeConverter.CanConvertTo(destinationType))
            {
                try
                {
                    result = typeConverter.ConvertTo(value, destinationType);
                    return true;
                }
                // ReSharper disable CatchAllClause
                catch
                // ReSharper restore CatchAllClause
                {
                }
            }

            typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (typeConverter.CanConvertFrom(valueType))
            {
                try
                {
                    result = typeConverter.ConvertFrom(value);
                    return true;
                }
                // ReSharper disable CatchAllClause
                catch
                // ReSharper restore CatchAllClause
                {
                }
            }
            result = null;
            return false;
        }

        private static bool ValueRepresentsNull(object value)
        {
            return value == null || value == DBNull.Value;
        }
    }
}
