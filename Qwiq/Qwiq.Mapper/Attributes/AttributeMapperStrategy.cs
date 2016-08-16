using FastMember;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class AttributeMapperStrategy : IndividualWorkItemMapperBase
    {
        private readonly IPropertyInspector _inspector;
        private readonly ITypeParser _typeParser;
        private static readonly ConcurrentDictionary<PropertyInfo, string> PropertyInfoFields = new ConcurrentDictionary<PropertyInfo, string>();
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>> PropertiesThatExistOnWorkItem = new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>();

        public AttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser)
        {
            _inspector = inspector;
            _typeParser = typeParser;
        }

        private static string PropertyInfoFieldCache(IPropertyInspector inspector, PropertyInfo property)
        {
            return PropertyInfoFields.GetOrAdd(
                property,
                info =>
                    {
                        var a = inspector.GetAttribute<FieldDefinitionAttribute>(property);
                        return a?.GetFieldName();
                    });
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
                                         new { property, fieldName = PropertyInfoFieldCache(inspector, property) })
                                     .Where(
                                         @t =>
                                         !string.IsNullOrEmpty(@t.fieldName) && workItem.Fields.Contains(@t.fieldName))
                                     .Select(@t => @t.property)
                                     .ToList();
                    });
        }

        protected override void Map(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper)
        {
            var accessor = TypeAccessor.Create(targetWorkItemType, true);
            MapImpl(targetWorkItemType, sourceWorkItem, accessor, targetWorkItem);
        }

        public override void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, object>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var accessor = TypeAccessor.Create(targetWorkItemType, true);

            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, accessor, targetWorkItem);
            }
        }

        private void MapImpl(Type targetWorkItemType, IWorkItem sourceWorkItem, TypeAccessor accessor, object targetWorkItem)
        {
            foreach (var property in PropertiesOnWorkItemCache(_inspector, sourceWorkItem, targetWorkItemType, typeof(FieldDefinitionAttribute)))
            {
                var fieldName = PropertyInfoFieldCache(_inspector, property);

                try
                {
                    var value = ParseValue(property, sourceWorkItem[fieldName]);
                    accessor[targetWorkItem, property.Name] = value;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "Could not map field '{0}' from type '{1}' to type '{2}'. {3}",
                        fieldName,
                        sourceWorkItem.Type.Name,
                        targetWorkItemType.Name,
                        e.Message);
                }
            }
        }

        protected virtual object ParseValue(PropertyInfo property, object value)
        {
            return _typeParser.Parse(property.PropertyType, value);
        }
    }
}