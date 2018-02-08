using JetBrains.Annotations;
using System;
using System.Diagnostics.Contracts;

namespace Qwiq
{
    /// <summary>
    /// </summary>
    public class FieldDefinition : IFieldDefinition, IEquatable<IFieldDefinition>
    {
        internal FieldDefinition(int id, [NotNull] string referenceName, [NotNull] string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(referenceName));
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = string.Intern(name);
            ReferenceName = string.Intern(referenceName);

            if (id == 0)
            {
                if (!CoreFieldRefNames.CoreFieldIdLookup.TryGetValue(referenceName, out int fid))
                    fid = FieldDefinitionComparer.Default.GetHashCode(this);
                Id = fid;
            }
            else
            {
                Id = id;
            }
        }

        internal FieldDefinition([NotNull] string referenceName, [NotNull] string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(referenceName));
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = string.Intern(name);
            ReferenceName = string.Intern(referenceName);

            if (!CoreFieldRefNames.CoreFieldIdLookup.TryGetValue(referenceName, out int id))
                id = FieldDefinitionComparer.Default.GetHashCode(this);

            Id = id;
        }

        public int Id { get; }

        public string Name { get; }

        public string ReferenceName { get; }

        public bool Equals(IFieldDefinition other)
        {
            return FieldDefinitionComparer.Default.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return FieldDefinitionComparer.Default.Equals(this, obj as IFieldDefinition);
        }

        public override int GetHashCode()
        {
            return FieldDefinitionComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            return ReferenceName;
        }
    }
}