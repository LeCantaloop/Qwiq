using System;

namespace Microsoft.IE.Qwiq.Mapper
{
    public interface ITypeParser
    {
        object Parse(Type destinationType, object value, object defaultValue);
        object Parse(Type destinationType, object input);
        T Parse<T>(object value);
        T Parse<T>(object value, T defaultValue);
    }
}