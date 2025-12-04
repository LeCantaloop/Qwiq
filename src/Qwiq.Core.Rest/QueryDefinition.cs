using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryDefinition : Qwiq.QueryDefinition
    {
        internal QueryDefinition(QueryHierarchyItem queryDefinition)
            : base(
                (queryDefinition ?? throw new ArgumentNullException(nameof(queryDefinition))).Id,
                queryDefinition.Name,
                queryDefinition.Wiql,
                queryDefinition.Path)
        {
        }
    }
}