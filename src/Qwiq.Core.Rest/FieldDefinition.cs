using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition(WorkItemFieldReference field)
            :base(field.ReferenceName, field.Name)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
        }
    }
}