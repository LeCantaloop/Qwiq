using System;

namespace Qwiq.Client.Soap
{
    internal class QueryDefinition : Qwiq.QueryDefinition
    {
        internal QueryDefinition(Microsoft.TeamFoundation.WorkItemTracking.Client.QueryDefinition queryDefinition)
                : base(
                    (queryDefinition ?? throw new ArgumentNullException(nameof(queryDefinition))).Id,
                    queryDefinition.Name,
                    queryDefinition.QueryText,
                    queryDefinition.Path)
        {
        }
    }
}
