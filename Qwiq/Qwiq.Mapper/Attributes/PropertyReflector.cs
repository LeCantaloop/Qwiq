using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class PropertyReflector : PropertyReflectorBase
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<PropertyInfo, IEnumerable<Attribute>> PropertyAttributes = new ConcurrentDictionary<PropertyInfo, IEnumerable<Attribute>>();

        public override IEnumerable<PropertyInfo> GetProperties(Type workItemType)
        {
            return TypePropertiesCache(workItemType);
        }

        public override IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property)
        {
            return CustomAttributesCache(property);
        }

        public override IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType)
        {
            var attributes = GetCustomAttributes(property);
            return FilterAttributes(attributes, attributeType);
        }

        public override Attribute GetAttribute(Type type, PropertyInfo property)
        {
            var attributes = GetCustomAttributes(property);
            return FindFirstAttribute(attributes, type);
        }

        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis;
            }

            var properties = type.GetProperties().ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        private static IEnumerable<Attribute> CustomAttributesCache(PropertyInfo property)
        {
            IEnumerable<Attribute> @as;
            if (PropertyAttributes.TryGetValue(property, out @as))
            {
                return @as;
            }

            @as = property.GetCustomAttributes(true).Cast<Attribute>().ToArray();
            PropertyAttributes[property] = @as;
            return @as;
        }
    }
}
