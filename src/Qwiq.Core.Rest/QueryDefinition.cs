using JetBrains.Annotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryDefinition : Qwiq.QueryDefinition
    {
        internal QueryDefinition([NotNull] QueryHierarchyItem queryDefinition)
            : base(queryDefinition.Id, queryDefinition.Name, queryDefinition.Wiql)
        {
        }
    }
}