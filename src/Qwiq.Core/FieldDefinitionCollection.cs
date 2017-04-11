using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public abstract class FieldDefinitionCollection : ReadOnlyListWithId<IFieldDefinition, int>,
                                                      IFieldDefinitionCollection
    {
        protected internal FieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
            : base(fieldDefinitions, definition => definition.Name)
        {
        }

        public bool Equals(IFieldDefinitionCollection other)
        {
            return FieldDefinitionCollectionComparer.Instance.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return FieldDefinitionCollectionComparer.Instance.Equals(this, obj as IFieldDefinitionCollection);
        }

        public override int GetHashCode()
        {
            return FieldDefinitionCollectionComparer.Instance.GetHashCode(this);
        }

        protected override void Add(IFieldDefinition value, int index)
        {
            base.Add(value, index);
            AddByName(value.ReferenceName, index);
        }
    }
}