using FastMember;
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
        private readonly IPropertyInspector _inspector;
        private readonly ITypeParser _typeParser;
        private static readonly ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute> PropertyInfoFields = new ConcurrentDictionary<PropertyInfo, FieldDefinitionAttribute>();
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>> PropertiesThatExistOnWorkItem = new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>();

        public AttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser)
        {
            _inspector = inspector;
            _typeParser = typeParser;
        }

        private static FieldDefinitionAttribute PropertyInfoFieldCache(IPropertyInspector inspector, PropertyInfo property)
        {
            return PropertyInfoFields.GetOrAdd(
                property,
                info => inspector.GetAttribute<FieldDefinitionAttribute>(property));
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

        public override void Map<T>(IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var targetWorkItemType = typeof(T);
            var accessor = TypeAccessor.Create(targetWorkItemType, true);

            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, accessor, targetWorkItem);
            }
        }

        public override void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> workItemMappings, IWorkItemMapper workItemMapper)
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
#if DEBUG
            Trace.TraceInformation("{0}: Mapping {1}", GetType().Name, sourceWorkItem.Id);
#endif
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

                try
                {
                    if (convert)
                    {
                        try
                        {
                            fieldValue = _typeParser.Parse(property.PropertyType, fieldValue, nullSub);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Trace.TraceWarning(
                                    "Could not convert value of field '{0}' ({1}) to type {2}",
                                    fieldName,
                                    fieldValue.GetType().Name,
                                    property.PropertyType.Name);
                            }
                            catch (Exception)
                            {
                                // Best effort
                            }
                        }
                    }

                    if (fieldValue == null && nullSub != null)
                    {
                        fieldValue = nullSub;
                    }

                    accessor[targetWorkItem, property.Name] = fieldValue;

                }
                catch(NullReferenceException) when (fieldValue == null)
                {
                    // This is most likely the cause of the field being null and the target property type not accepting nulls
                    // For example: mapping null to an int instead of int?

                    try
                    {
                        Trace.TraceWarning(
                            "Could not map field '{0}' from type '{1}' to type '{2}'. Target '{2}.{3}' does not accept null values.",
                            fieldName,
                            sourceWorkItem.Type.Name,
                            targetWorkItemType.Name,
                            $"{property.Name} ({property.PropertyType.FullName})");
                    }
                    catch (Exception)
                    {
                        // Best effort
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        Trace.TraceWarning(
                            "Could not map field '{0}' from type '{1}' to type '{2}'. {3}",
                            fieldName,
                            sourceWorkItem.Type.Name,
                            targetWorkItemType.Name,
                            e.Message);
                    }
                    catch (Exception)
                    {
                        // Best effort
                    }
                }
            }
        }
    }
}
