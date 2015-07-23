using System;
using System.Reflection;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class AttributeMapperStrategy : IWorkItemMapperStrategy
    {
        private readonly IPropertyInspector _inspector;
        private readonly ITypeParser _typeParser;

        public AttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser)
        {
            _inspector = inspector;
            _typeParser = typeParser;
        }

        public void Map(Type targeWorkItemType, Qwiq.IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper)
        {
            var properties = _inspector.GetAnnotatedProperties(targeWorkItemType, typeof(FieldDefinitionAttribute));
            foreach (var property in properties)
            {
                var field = _inspector.GetAttribute<FieldDefinitionAttribute>(property);
                if (field != null)
                {
                    var value = ParseValue(property, sourceWorkItem[field.GetFieldName()]);
                    property.SetValue(targetWorkItem, value);
                }
            }
        }

        protected virtual object ParseValue(PropertyInfo property, object value)
        {
            return _typeParser.Parse(property.PropertyType, value);
        }
    }
}