using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class FieldDefinitionProxy
    {
        internal FieldDefinitionProxy(WorkItemFieldReference field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            Name = field.Name;
            ReferenceName = field.ReferenceName;
        }
    }
}