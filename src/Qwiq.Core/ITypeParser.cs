using System;
using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public interface ITypeParser
    {
        object Parse([CanBeNull] Type destinationType, [CanBeNull] object value, [CanBeNull] object defaultValue);
        object Parse([CanBeNull] Type destinationType, [CanBeNull] object input);
        T Parse<T>([CanBeNull] object value);
        T Parse<T>([CanBeNull] object value, [CanBeNull] T defaultValue);
    }
}
