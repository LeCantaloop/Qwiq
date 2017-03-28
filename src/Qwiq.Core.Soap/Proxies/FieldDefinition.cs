using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition(Tfs.FieldDefinition fieldDefinition)
            :base(fieldDefinition?.ReferenceName, fieldDefinition?.Name)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
        }
    }
}