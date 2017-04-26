using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        public FieldDefinitionCollection(IWorkItemStore store)
            : base(store?.Projects.SelectMany(s => s.WorkItemTypes).SelectMany(s => s.FieldDefinitions).Select(s => s))
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
        }

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