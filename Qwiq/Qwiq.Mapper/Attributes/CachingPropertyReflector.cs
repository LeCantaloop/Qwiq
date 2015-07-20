using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class CachingPropertyReflector : PropertyReflectorBase
    {
        private readonly IPropertyReflector _innerReflector;
        private readonly ConcurrentDictionary<string, object> _cache;

        public CachingPropertyReflector(IPropertyReflector innerReflector)
        {
            _innerReflector = innerReflector;
            _cache = new ConcurrentDictionary<string, object>();
        }

        public override IEnumerable<PropertyInfo> GetProperties(Type workItemType)
        {
            return GetOrAdd(GenerateCacheKey("GetProperties", workItemType),
                () => _innerReflector.GetProperties(workItemType));
        }

        public override IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property)
        {
            return GetOrAdd(GenerateCacheKey("GetCustomAttributes", null, property),
                () => _innerReflector.GetCustomAttributes(property));
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

        private T GetOrAdd<T>(string key, Func<T> func)
        {
            return (T)_cache.GetOrAdd(key, val => func());
        }

        private string GenerateCacheKey(string methodName, Type type = null, PropertyInfo propertyInfo = null)
        {
            var key = methodName;
            if (type != null)
            {
                key += type.AssemblyQualifiedName;
            }
            if (propertyInfo != null)
            {
                key += propertyInfo.GetHashCode();
            }
            return key;
        }
    }
}