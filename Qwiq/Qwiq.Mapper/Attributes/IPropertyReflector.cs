using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public interface IPropertyReflector
    {
        IEnumerable<PropertyInfo> GetProperties(Type workItemType);
        IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property);
        IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType);
        object GetAttribute(Type type, PropertyInfo property);
    }
}