using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class FieldDefinitionProxy : Microsoft.Qwiq.Proxies.FieldDefinitionProxy
    {
        internal FieldDefinitionProxy(Tfs.FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            Name = fieldDefinition.Name;
            ReferenceName = fieldDefinition.ReferenceName;
        }
    }
}