using System;

namespace Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldDefinitionAttribute : Attribute
    {
        /// <exception cref="ArgumentException">Value for <paramref name="name"/> cannot be null, empty, or only whitespace.</exception>
        public FieldDefinitionAttribute(string name)
            : this(name, false, null)
        {
        }

        /// <exception cref="ArgumentException">Value for <paramref name="name"/> cannot be null, empty, or only whitespace.</exception>
        public FieldDefinitionAttribute(string name, bool requireConversion)
            : this(name, requireConversion, null)
        {
        }

        /// <exception cref="ArgumentException">Value for <paramref name="name"/> cannot be null, empty, or only whitespace.</exception>
        public FieldDefinitionAttribute(string name, object nullSubstitute)
            : this(name, false, nullSubstitute)
        {
        }

        /// <exception cref="ArgumentException">Value for <paramref name="name"/> cannot be null, empty, or only whitespace.</exception>
        public FieldDefinitionAttribute(string name, bool requireConversion, object nullSubstitute)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            FieldName = name;
            RequireConversion = requireConversion;
            NullSubstitute = nullSubstitute;
        }

        public string FieldName { get; }

        public object NullSubstitute { get; }

        public bool RequireConversion { get; }
    }
}