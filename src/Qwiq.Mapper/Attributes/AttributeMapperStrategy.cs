using FastMember;
using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class AttributeMapperStrategy : WorkItemMapperStrategyBase
    {
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>> PropertiesThatExistOnWorkItem = new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>();
        private static readonly ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute> PropertyInfoFields = new ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute>();
        [NotNull] private readonly IPropertyInspector _inspector;
        [NotNull] private readonly ITypeParser _typeParser;


        public AttributeMapperStrategy([NotNull] IPropertyInspector inspector)
            : this(inspector, TypeParser.Default)
        {
        }

        public AttributeMapperStrategy([NotNull] IPropertyInspector inspector, [NotNull] ITypeParser typeParser)
        {
            _typeParser = typeParser ?? throw new ArgumentNullException(nameof(typeParser));
            _inspector = inspector ?? throw new ArgumentNullException(nameof(inspector));
        }

        public override void Map<T>(IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var targetWorkItemType = typeof(T);

            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, targetWorkItem);
            }
        }

        public override void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, targetWorkItem);
            }
        }

        protected internal virtual void AssignFieldValue(
            [NotNull] Type targetWorkItemType,
            [NotNull] IWorkItem sourceWorkItem,
            [NotNull] object targetWorkItem,
            [NotNull] PropertyInfo property,
            [NotNull] string fieldName,
            bool convert,
            [CanBeNull] object nullSub,
            [CanBeNull] object fieldValue)
        {
            // Coalesce fieldValue and nullSub

            if (fieldValue == null && nullSub != null)
            {
                fieldValue = nullSub;
            }
            else
            {
                if (fieldValue is string value && string.IsNullOrWhiteSpace(value))
                    fieldValue = nullSub;
            }

            var destType = property.PropertyType;

            if (fieldValue == null && destType.IsValueType)
            {
                // Value types do not accept null; don't do any work
                return;
            }

            if (fieldValue == null && destType.CanAcceptNull())
            {
                // Destination is a nullable or can take null; don't do any work
                return;
            }

            var sourceType = fieldValue.GetType();
            if (convert)
            {
                try
                {
                    fieldValue = _typeParser.Parse(destType, fieldValue);
                }
                catch (Exception e)
                {
                    throw new AttributeMapException($"Unable to convert field {fieldName} on work item {sourceWorkItem.Id}: {sourceWorkItem.WorkItemType} to property {property.Name} on {targetWorkItemType}.", e);
                }
            }

            var accessor = TypeAccessor.Create(targetWorkItemType, true);
            accessor[targetWorkItem, property.Name] = fieldValue;
        }

        protected internal virtual void MapImpl(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem)
        {
            var properties = PropertiesOnWorkItemCache(
                _inspector,
                sourceWorkItem,
                targetWorkItemType,
                typeof(FieldDefinitionAttribute));

            foreach (var property in properties)
            {
                var a = PropertyInfoFieldCache(_inspector, property);
                if (a == null) continue;

                var fieldName = a.FieldName;
                var convert = a.RequireConversion;
                var nullSub = a.NullSubstitute;
                var fieldValue = sourceWorkItem[fieldName];

                AssignFieldValue(targetWorkItemType, sourceWorkItem, targetWorkItem, property, fieldName, convert, nullSub, fieldValue);
            }
        }

        private static IEnumerable<PropertyInfo> PropertiesOnWorkItemCache(IPropertyInspector inspector, IWorkItem workItem, Type targetType, Type attributeType)
        {
            // Composite key: work item type and target type

            var workItemType = workItem.Type.Name;
            var key = new Tuple<string, RuntimeTypeHandle>(workItemType, targetType.TypeHandle);

            return PropertiesThatExistOnWorkItem.GetOrAdd(
                key,
                tuple =>
                    {
                        return
                            inspector.GetAnnotatedProperties(targetType, typeof(FieldDefinitionAttribute))
                                     .Select(
                                         property =>
                                         new { property, fieldName = PropertyInfoFieldCache(inspector, property)?.FieldName })
                                     .Where(
                                         @t =>
                                         !string.IsNullOrEmpty(@t.fieldName) && workItem.Fields.Contains(@t.fieldName))
                                     .Select(@t => @t.property)
                                     .ToList();
                    });
        }

        private static FieldDefinitionAttribute PropertyInfoFieldCache(IPropertyInspector inspector, PropertyInfo property)
        {
            return PropertyInfoFields.GetOrAdd(
                property,
                info => inspector.GetAttribute<FieldDefinitionAttribute>(property));
        }
    }
}