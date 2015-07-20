using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class PropertyInspector : IPropertyInspector
    {
        private readonly IPropertyReflector _reflector;

        public PropertyInspector(IPropertyReflector reflector)
        {
            _reflector = reflector;
        }

        public IEnumerable<PropertyInfo> GetAnnotatedProperties(Type workItemType, Type attributeType)
        {
            return _reflector.GetProperties(workItemType).Where(prop => _reflector.GetCustomAttributes(prop, attributeType).Any());
        }

        public T GetAttribute<T>(PropertyInfo property)
        {
            return (T)_reflector.GetAttribute(typeof(T), property);
        }
    }
}
