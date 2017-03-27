using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class FieldDefinitionProxy : IFieldDefinition
    {
        private readonly WorkItemFieldReference _field;

        internal FieldDefinitionProxy(WorkItemFieldReference field)
        {
            _field = field;
        }

        public string Name => _field.Name;

        public string ReferenceName => _field.ReferenceName;

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