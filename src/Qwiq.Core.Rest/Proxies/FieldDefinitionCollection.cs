using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        internal FieldDefinitionCollection(IEnumerable<WorkItemTypeFieldInstance> typeFields)
            : this(typeFields.Where(p => p != null).Select(s => s.Field))
        {
        }

        internal FieldDefinitionCollection(IEnumerable<WorkItemFieldReference> typeFields)
            : this(typeFields.Where(p => p != null).Select(s => new FieldDefinition(s)))
        {
        }

        private FieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
            : base(fieldDefinitions)
        {
        }
    }
}