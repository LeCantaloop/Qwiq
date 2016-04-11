using System;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class AttributeMapperStrategy : IndividualWorkItemMapperBase
    {
        private readonly IPropertyInspector _inspector;
        private readonly ITypeParser _typeParser;

        public AttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser)
        {
            _inspector = inspector;
            _typeParser = typeParser;
        }

        protected override void Map(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper)
        {
            var properties = _inspector.GetAnnotatedProperties(targetWorkItemType, typeof(FieldDefinitionAttribute));
            foreach (var property in properties)
            {
                var field = _inspector.GetAttribute<FieldDefinitionAttribute>(property);
                if (field != null)
                {
                    var fieldName = field.GetFieldName();
                    try
                    {
                        // This check is to see if a field exists before attempting to access it. This allows us to
                        // ignore properties which have an attributed field which does not have a backed field in the IWorkItem source
                        // rather than throw an exception.
                        if (sourceWorkItem.Fields.Contains(fieldName))
                        {
                            var value = ParseValue(property, sourceWorkItem[fieldName]);
                            property.SetValue(targetWorkItem, value);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.TraceWarning("Could not map field '{0}' from type '{1}' to type '{2}'. {3}",
                            fieldName,
                            sourceWorkItem.Type.Name,
                            targetWorkItemType.Name,
                            e.Message);
                    }
                }
            }
        }

        protected virtual object ParseValue(PropertyInfo property, object value)
        {
            return _typeParser.Parse(property.PropertyType, value);
        }
    }
}