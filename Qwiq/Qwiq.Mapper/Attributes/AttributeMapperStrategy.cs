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

        protected override void Map(Type targetWorkItemType, Qwiq.IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper)
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
                        var value = ParseValue(property, sourceWorkItem[fieldName]);
                        property.SetValue(targetWorkItem, value);
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