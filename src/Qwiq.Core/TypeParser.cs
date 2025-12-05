using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Qwiq
{
    /// <summary>
    ///     Helper class that converts objects (as they are returned from the TFS API) to usable types.
    /// </summary>
    public class TypeParser : ITypeParser
    {
        private static readonly Hashtable TypeConverters = new Hashtable();

        private TypeParser()
        {
        }

        public static ITypeParser Default => Nested.Instance;

        public object? Parse(Type destinationType, object? value, object? defaultValue)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            var defaultValueType = defaultValue?.GetType();
            if (defaultValueType != null && destinationType != defaultValueType)
            {
                Trace.TraceWarning($"The type of parameter {nameof(defaultValue)} ({defaultValueType}) does not match the destination type ({destinationType}. An additional conversion is required to convert to {destinationType}.");
            }

            return ParseImpl(destinationType, value, defaultValue);
        }

        public object? Parse(Type destinationType, object? input)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            return ParseImpl(destinationType, input);
        }

        public T Parse<T>(object? value)
        {
            return Parse(value, default(T)!);
        }

        public T Parse<T>(object? value, T defaultValue)
        {
            return (T)Parse(typeof(T), value, defaultValue)!;
        }
        private static object? ParseImpl(Type destinationType, object? value)
        {
            var valueIsNull = ValueRepresentsNull(value);
            var canAcceptNull = destinationType.CanAcceptNull();

            if (valueIsNull)
            {
                if (canAcceptNull)
                    return null;

                return destinationType.GetDefaultValueOfType();
            }

            var valueType = value!.GetType();

            // Quit if no type conversion is actually required
            if (valueType == destinationType) return value;
            if (destinationType.IsInstanceOfType(value)) return value;

            switch (Type.GetTypeCode(destinationType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    var destNullable = destinationType.IsGenericNullable();
                    if (!destNullable && valueType == typeof(string))
                    {
                        var val = (string)value;
                        if (string.IsNullOrEmpty(val))
                        {
                            return destinationType.GetDefaultValueOfType();
                        }
                    }
                    break;
            }

            if (TryConvert(destinationType, value, out object? result)) return result;

            var defaultValue = destinationType.GetDefaultValueOfType();
            if (destinationType.IsGenericNullable() && defaultValue == null) return null;

            if (defaultValue != null)
            {
                var defaultValueType = defaultValue.GetType();
                if (defaultValueType == destinationType)
                {
                    // No conversion required
                    return defaultValue;
                }
            }

            if (TryConvert(destinationType, defaultValue, out result)) return result;

            return null;
        }
        private static object? ParseImpl(
            Type destinationType,
            object? value,
            object? defaultValue)
        {
            var valueIsNull = ValueRepresentsNull(value);
            var defaultValueIsNull = ValueRepresentsNull(defaultValue);
            var canAcceptNull = destinationType.CanAcceptNull();

            if (valueIsNull)
            {
                if (defaultValueIsNull)
                {
                    if (canAcceptNull)
                        return null;

                    // A value type cannot have a null return
                    throw new InvalidOperationException($"The type {destinationType} cannot have a null value.", new ArgumentNullException(nameof(value), new ArgumentNullException(nameof(defaultValue))));
                }

                // If the incoming value is null, return the default value
                return defaultValue;
            }

            var valueType = value!.GetType();

            // Quit if no type conversion is actually required
            if (valueType == destinationType) return value;
            if (destinationType.IsInstanceOfType(value)) return value;

            switch (Type.GetTypeCode(destinationType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    var destNullable = destinationType.IsGenericNullable();
                    if (!destNullable && valueType == typeof(string))
                    {
                        var val = (string)value;
                        if (string.IsNullOrEmpty(val))
                        {
                            if (defaultValueIsNull)
                            {
                                return destinationType.GetDefaultValueOfType();
                            }
                            else
                            {
                                return defaultValue;
                            }
                        }
                    }
                    break;
            }

            if (TryConvert(destinationType, value, out object? result)) return result;
            if (destinationType.IsGenericNullable() && defaultValue == null) return null;

            if (defaultValue != null)
            {
                var defaultValueType = defaultValue.GetType();
                if (defaultValueType == destinationType)
                {
                    // No conversion required
                    return defaultValue;
                }
            }

            if (TryConvert(destinationType, defaultValue, out result)) return result;

            return null;
        }

        private static bool TryConvert(Type destinationType, object? value, out object? result)
        {
            if (value == null)
            {
                result = null;
                return destinationType.CanAcceptNull();
            }

            if (destinationType.IsGenericNullable())
                try
                {
                    var converter = new NullableConverter(destinationType);
                    result = converter.ConvertTo(value, converter.UnderlyingType);
                    return true;
                }
                catch
                {
                    // Intentionally empty: NullableConverter may fail for certain value types.
                    // Fall through to try other conversion strategies.
                }

            var valueType = value.GetType();
            var typeConverter = GetTypeConverter(valueType);

            if (typeConverter.CanConvertTo(destinationType))
                try
                {
                    result = typeConverter.ConvertTo(value, destinationType);
                    return true;
                }
                catch
                {
                    // Intentionally empty: TypeConverter.CanConvertTo may return true but still fail.
                    // Fall through to try other conversion strategies.
                }

            typeConverter = GetTypeConverter(destinationType);
            if (typeConverter.CanConvertFrom(valueType))
                try
                {
                    result = typeConverter.ConvertFrom(value);
                    return true;
                }
                catch
                {
                    // Intentionally empty: TypeConverter.CanConvertFrom may return true but still fail.
                    // Fall through to try other conversion strategies.
                }

            if (value != null)
            {
                var val = value.ToString();
                if (!string.IsNullOrEmpty(val))
                    if (typeConverter.IsValid(val))
                        try
                        {
                            result = typeConverter.ConvertFromString(val);
                            return true;
                        }
                        catch
                        {
                            // Intentionally empty: ConvertFromString may fail even when IsValid returns true.
                            // Fall through to return null result.
                        }
            }

            result = null;
            return false;
        }

        private static TypeConverter GetTypeConverter(Type valueType)
        {
            var hashtable = TypeConverters;

            var typeConverter = (TypeConverter?)hashtable[valueType];
            if (typeConverter != null) return typeConverter;

            lock (hashtable)
            {
                typeConverter = TypeDescriptor.GetConverter(valueType);
                hashtable[valueType] = typeConverter;
            }
            return typeConverter;
        }

        private static bool ValueRepresentsNull(object? value)
        {
            return value == null || value == DBNull.Value;
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
        // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly ITypeParser Instance = new TypeParser();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}