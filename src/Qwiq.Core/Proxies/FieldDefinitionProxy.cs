using System;

namespace Microsoft.Qwiq.Proxies
{
    public class FieldDefinitionProxy : IFieldDefinition, IComparable<IFieldDefinition>, IEquatable<IFieldDefinition>
    {
        public string Name { get; internal set; }

        public string ReferenceName { get; internal set; }

        public int CompareTo(IFieldDefinition other)
        {
            return FieldDefinitionComparer.Instance.Compare(this, other);
        }

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
    }
}