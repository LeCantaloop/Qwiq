using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Qwiq.Client.Soap
{
    internal class QueryDefinition : Qwiq.QueryDefinition
    {
        internal QueryDefinition([NotNull] Microsoft.TeamFoundation.WorkItemTracking.Client.QueryDefinition queryDefinition)
                : base(queryDefinition.Id, queryDefinition.Name, queryDefinition.QueryText, queryDefinition.Path)
        {
        }
    }
}
