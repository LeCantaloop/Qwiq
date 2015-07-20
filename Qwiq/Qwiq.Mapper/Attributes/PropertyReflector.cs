using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class PropertyReflector : PropertyReflectorBase
    {
        public override IEnumerable<PropertyInfo> GetProperties(Type workItemType)
        {
            return workItemType.GetProperties();
        }

        public override IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property)
        {
            return property.GetCustomAttributes(false).Cast<Attribute>();
        }

        public override IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType)
        {
            var attributes = GetCustomAttributes(property);
            return FilterAttributes(attributes, attributeType);
        }

        public override object GetAttribute(Type type, PropertyInfo property)
        {
            var attributes = GetCustomAttributes(property);
            return FindFirstAttribute(attributes, type);
        }
    }
}
