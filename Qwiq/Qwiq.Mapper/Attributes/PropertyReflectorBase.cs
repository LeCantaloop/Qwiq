using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public abstract class PropertyReflectorBase : IPropertyReflector
    {
        protected IEnumerable<Attribute> FilterAttributes(IEnumerable<Attribute> attributes, Type type)
        {
            return attributes.Where(attribute => attribute.GetType() == type);
        }

        protected object FindFirstAttribute(IEnumerable<Attribute> attributes, Type type)
        {
            return attributes.FirstOrDefault(attribute => attribute.GetType() == type);
        }

        public abstract IEnumerable<PropertyInfo> GetProperties(Type workItemType);
        public abstract IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property);
        public abstract IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType);
        public abstract object GetAttribute(Type type, PropertyInfo property);
    }
}
