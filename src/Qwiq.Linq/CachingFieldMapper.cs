using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Qwiq.Linq
{
    public class CachingFieldMapper : IFieldMapper
    {
        private readonly IFieldMapper _innerMapper;
        private readonly ConcurrentDictionary<string, object> _cache;

        public CachingFieldMapper([NotNull] IFieldMapper innerMapper)
        {
            Contract.Requires(innerMapper != null);

            _innerMapper = innerMapper ?? throw new ArgumentNullException(nameof(innerMapper));
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

        private string GenerateCacheKey([NotNull] Type type, [NotNull] string method, [NotNull] string propertyName = "")
        {
            Contract.Requires(type != null);
            Contract.Requires(method != null);
            Contract.Requires(propertyName != null);

            return type.AssemblyQualifiedName + method + propertyName;
        }
    }
}
