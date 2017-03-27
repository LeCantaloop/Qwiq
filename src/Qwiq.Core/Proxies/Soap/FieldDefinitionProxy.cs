using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class FieldDefinitionProxy
    {
        internal FieldDefinitionProxy(Tfs.FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            Name = fieldDefinition.Name;
            ReferenceName = fieldDefinition.ReferenceName;
        }
    }
}