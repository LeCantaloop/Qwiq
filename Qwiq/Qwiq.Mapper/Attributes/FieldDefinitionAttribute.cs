using System;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldDefinitionAttribute : Attribute
    {
        public FieldDefinitionAttribute(string name)
            : this(name, false)
        {

        }

        public FieldDefinitionAttribute(string name, bool requireConversion)
        {
            FieldName = name;
            RequireConversion = requireConversion;
        }

        public string FieldName { get; }

        public bool RequireConversion { get; }
    }
}
