using System;

namespace Microsoft.Qwiq.Mapper
{
    public interface ITypeParser
    {
        object Parse(Type destinationType, object value, object defaultValue);
        object Parse(Type destinationType, object input);
        T Parse<T>(object value);
        T Parse<T>(object value, T defaultValue);
    }
}
