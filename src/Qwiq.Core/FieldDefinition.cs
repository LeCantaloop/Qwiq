using System;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// A facade for <see cref="Microsoft.TeamFoundation.WorkItemTracking.Client.FieldDefinition"/>
    /// </summary>
    public class FieldDefinition : IFieldDefinition, IEquatable<IFieldDefinition>
    {
        internal FieldDefinition(int id, string referenceName, string name)
            : this(referenceName, name)
        {
            Id = id;
        }

        internal FieldDefinition(string referenceName, string name)
        {
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
            ReferenceName = referenceName;
            Id = FieldDefinitionComparer.Instance.GetHashCode(this);
        }

        public string Name { get; }

        public string ReferenceName { get; }

        public int Id { get; }

        public bool Equals(IFieldDefinition other)
        {
            return FieldDefinitionComparer.Instance.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return FieldDefinitionComparer.Instance.Equals(this, obj as IFieldDefinition);
        }

        public override int GetHashCode()
        {
            return FieldDefinitionComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return ReferenceName;
        }
    }
}