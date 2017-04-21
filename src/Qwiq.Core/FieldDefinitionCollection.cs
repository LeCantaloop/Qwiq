using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public abstract class FieldDefinitionCollection : ReadOnlyCollectionWithId<IFieldDefinition, int>, IFieldDefinitionCollection
    {
        protected internal FieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
            : base(fieldDefinitions, definition => definition.Name)
        {
        }

        public bool Equals(IFieldDefinitionCollection other)
        {
            return Comparer.FieldDefinitionCollection.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Comparer.FieldDefinitionCollection.Equals(this, obj as IFieldDefinitionCollection);
        }

        public override int GetHashCode()
        {
            return Comparer.FieldDefinitionCollection.GetHashCode(this);
        }

        protected override void Add(IFieldDefinition value, int index)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            base.Add(value, index);
            AddByName(value.ReferenceName, index);
        }
    }
}