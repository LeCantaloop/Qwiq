using System;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FieldDefinitionAttribute : Attribute
    {
        private readonly string _name;

        public FieldDefinitionAttribute(string name)
        {
            _name = name;
        }

        public string GetFieldName()
        {
            return _name;
        }
    }
}
