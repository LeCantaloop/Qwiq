using System;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// </summary>
    public class FieldDefinition : IFieldDefinition, IEquatable<IFieldDefinition>
    {
        internal FieldDefinition(int id, string referenceName, string name)

        {
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
            ReferenceName = referenceName;

            if (id == 0)
            {
                if (!CoreFieldRefNames.CoreFieldIdLookup.TryGetValue(referenceName, out int fid))
                    fid = FieldDefinitionComparer.Instance.GetHashCode(this);
                Id = fid;
            }
            else
            {
                Id = id;
            }
        }

        internal FieldDefinition(string referenceName, string name)
        {
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
            ReferenceName = referenceName;

            if (!CoreFieldRefNames.CoreFieldIdLookup.TryGetValue(referenceName, out int id))
                id = FieldDefinitionComparer.Instance.GetHashCode(this);

            Id = id;
        }

        public bool Equals(IFieldDefinition other)
        {
            return FieldDefinitionComparer.Instance.Equals(this, other);
        }

        public int Id { get; }

        public string Name { get; }

        public string ReferenceName { get; }

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