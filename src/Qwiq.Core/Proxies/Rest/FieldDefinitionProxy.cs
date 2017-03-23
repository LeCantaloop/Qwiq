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


    }
}