using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public abstract class PropertyReflectorBase : IPropertyReflector
    {
        protected IEnumerable<Attribute> FilterAttributes(IEnumerable<Attribute> attributes, Type type)
        {
            return attributes.Where(attribute => attribute.GetType() == type);
        }

        protected Attribute FindFirstAttribute(IEnumerable<Attribute> attributes, Type type)
        {
            return FilterAttributes(attributes, type).FirstOrDefault();
        }

        public abstract IEnumerable<PropertyInfo> GetProperties(Type workItemType);
        public abstract IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property);
        public abstract IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType);
        public abstract Attribute GetAttribute(Type type, PropertyInfo property);
    }
}

