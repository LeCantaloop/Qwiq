using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class FieldDefinitionProxy : IFieldDefinition
    {
        private readonly Tfs.FieldDefinition _fieldDefinition;

        public FieldDefinitionProxy(Tfs.FieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition ?? throw new ArgumentNullException(nameof(fieldDefinition));
        }

        public string Name => _fieldDefinition.Name;

        public string ReferenceName => _fieldDefinition.ReferenceName;

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