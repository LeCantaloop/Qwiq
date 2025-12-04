using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition(WorkItemFieldReference field)
            : base(
                (field ?? throw new ArgumentNullException(nameof(field))).ReferenceName,
                field.Name)
        {
        }
    }
}