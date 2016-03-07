using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Linq
{
    public class CachingFieldMapper : IFieldMapper
    {
        private readonly IFieldMapper _innerMapper;
        private readonly ConcurrentDictionary<string, object> _cache;

        public CachingFieldMapper(IFieldMapper innerMapper)
        {
            _innerMapper = innerMapper;
            _cache = new ConcurrentDictionary<string, object>();
        }

        public IEnumerable<string> GetWorkItemType(Type type)
        {
            return GetOrAdd(GenerateCacheKey(type, "GetWorkItemType"), () => _innerMapper.GetWorkItemType(type));
        }

        public IEnumerable<string> GetFieldNames(Type type)
        {
            return GetOrAdd(GenerateCacheKey(type, "GetFieldNames"), () => _innerMapper.GetFieldNames(type));
        }

        public string GetFieldName(Type type, string propertyName)
        {
            return GetOrAdd(GenerateCacheKey(type, "GetFieldName", propertyName),
                () => _innerMapper.GetFieldName(type, propertyName));
        }

        private T GetOrAdd<T>(string key, Func<T> func)
        {
            return (T)_cache.GetOrAdd(key, val => func());
        }

        private string GenerateCacheKey(Type type, string method, string propertyName = "")
        {
            return type.AssemblyQualifiedName + method + propertyName;
        }
    }
}