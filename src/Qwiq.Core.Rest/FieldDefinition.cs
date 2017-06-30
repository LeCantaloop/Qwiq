using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition([NotNull] WorkItemFieldReference field)
            :base(field.ReferenceName, field.Name)
        {
            Contract.Requires(field != null);

            if (field == null) throw new ArgumentNullException(nameof(field));
        }
    }
}