using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qwiq.Mapper.Attributes
{
    public class PropertyInspector : IPropertyInspector
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>> AnnotatedProperties = new ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>>();
        private readonly IPropertyReflector _reflector;

        public PropertyInspector([NotNull] IPropertyReflector reflector)
        {
            _reflector = reflector ?? throw new ArgumentNullException(nameof(reflector));
        }

        public IEnumerable<PropertyInfo> GetAnnotatedProperties(Type workItemType, Type attributeType)
        {
            return AnnotatedPropertiesCache(_reflector, workItemType, attributeType);
        }

        public T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return (T)_reflector.GetAttribute(typeof(T), property);
        }

        private static IEnumerable<PropertyInfo> AnnotatedPropertiesCache(IPropertyReflector reflector, Type workItemType, Type attributeType)
        {
            if (AnnotatedProperties.TryGetValue(workItemType.TypeHandle, out ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> f))
            {
                if (f.TryGetValue(attributeType.TypeHandle, out IEnumerable<PropertyInfo> pis))
                {
                    return pis;
                }

                pis = reflector
                       .GetProperties(workItemType)
                       .Where(prop => reflector.GetCustomAttributes(prop, attributeType).Any())
                       .ToArray();

                f[attributeType.TypeHandle] = pis;
                return pis;
            }

            var pis2 =
                reflector.GetProperties(workItemType)
                         .Where(prop => reflector.GetCustomAttributes(prop, attributeType).Any())
                         .ToArray();

            AnnotatedProperties[workItemType.TypeHandle] =
                new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>(
                    new[]
                        {
                            new KeyValuePair<RuntimeTypeHandle, IEnumerable<PropertyInfo>>(
                                attributeType.TypeHandle,
                                pis2
                                )
                        });

            return pis2;
        }
    }
}