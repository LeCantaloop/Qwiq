using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qwiq.Mapper.Attributes
{
    public interface IPropertyInspector
    {
        IEnumerable<PropertyInfo> GetAnnotatedProperties(Type workItemType, Type attributeType);

        T GetAttribute<T>(PropertyInfo property) where T : Attribute;
    }
}
