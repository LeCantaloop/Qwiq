using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class PropertyInspector : IPropertyInspector
    {
        private readonly IPropertyReflector _reflector;
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>> AnnotatedProperties = new ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>>();

        public PropertyInspector(IPropertyReflector reflector)
        {
            _reflector = reflector;
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
            ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> f;
            if (AnnotatedProperties.TryGetValue(workItemType.TypeHandle, out f))
            {
                IEnumerable<PropertyInfo> pis;
                if (f.TryGetValue(attributeType.TypeHandle, out pis))
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

