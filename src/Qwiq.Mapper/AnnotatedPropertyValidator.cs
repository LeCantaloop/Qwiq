using JetBrains.Annotations;
using Microsoft.Qwiq.Mapper.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper
{
    public class AnnotatedPropertyValidator : IAnnotatedPropertyValidator
    {
        private static readonly Type AttributeType = typeof(FieldDefinitionAttribute);
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, Dictionary<PropertyInfo, FieldDefinitionAttribute>> PropertiesThatExistOnWorkItem = new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, Dictionary<PropertyInfo, FieldDefinitionAttribute>>();
        private static readonly ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute> PropertyInfoFields = new ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute>();
        private readonly IPropertyInspector _inspector;
        private Func<IWorkItem, PropertyInfo, bool> _propertyInfoValidator;

        public AnnotatedPropertyValidator([NotNull] IPropertyInspector inspector)
        {
            _inspector = inspector ?? throw new ArgumentNullException(nameof(inspector));
            PropertyInfoValidator = (item, info) =>
            {
                var name = GetFieldDefinition(info)?.FieldName;
                var validName = !string.IsNullOrWhiteSpace(name);
                return validName && item.Fields.Contains(name);
            };
        }

        public Func<IWorkItem, PropertyInfo, bool> PropertyInfoValidator
        {
            get => _propertyInfoValidator;
            set
            {
                _propertyInfoValidator = value;
                PropertiesThatExistOnWorkItem.Clear();
            }
        }

        public FieldDefinitionAttribute GetFieldDefinition(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return PropertyInfoFields.GetOrAdd(
                property,
                info => _inspector.GetAttribute<FieldDefinitionAttribute>(property));
        }

        /// <exception cref="InvalidOperationException"><see cref="PropertyInfoValidator"/> is null.</exception>
        public IEnumerable<KeyValuePair<PropertyInfo, FieldDefinitionAttribute>> GetValidAnnotatedProperties(IWorkItem workItem, Type targetType)
        {
            if (PropertyInfoValidator == null)
                throw new InvalidOperationException($"{nameof(PropertyInfoValidator)} cannot be null.");

            var workItemTypeName = workItem.WorkItemType;
            var key = new Tuple<string, RuntimeTypeHandle>(workItemTypeName, targetType.TypeHandle);

            Dictionary<PropertyInfo, FieldDefinitionAttribute> ValueFactory(Tuple<string, RuntimeTypeHandle> tuple)
            {
                var props = _inspector.GetAnnotatedProperties(targetType, AttributeType);
                var retval = new Dictionary<PropertyInfo, FieldDefinitionAttribute>();
                foreach (var prop in props)
                {
                    if (prop == null) continue;
                    var attribute = GetFieldDefinition(prop);
                    if (attribute == null) continue;

                    if (PropertyInfoValidator(workItem, prop))
                    {
                        retval.Add(prop, attribute);
                    }
                }

                return retval;
            }

            return PropertiesThatExistOnWorkItem.GetOrAdd(
                key,
                ValueFactory
            );
        }
    }
}