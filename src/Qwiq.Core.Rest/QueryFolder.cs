using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryFolder : Qwiq.QueryFolder
    {
        internal QueryFolder(QueryHierarchyItem queryFolder, IQueryHierarchyItemRepository queryItemExpander)
            : base(
                queryFolder.Id,
                queryFolder.Name,
                queryFolder.Path,
                new QueryFolderCollection(() =>
                {
                    return
                        queryFolder
                            .Children?
                            .Where(q => q.IsFolder())
                            .Select(queryItemExpander.EnsureExpanded)
                            .Select(q => new QueryFolder(q, queryItemExpander))
                        ?? Enumerable.Empty<IQueryFolder>();
                }),
                new QueryDefinitionCollection(() =>
                {
                    return
                        queryFolder
                            .Children?
                            .Where(q => (q != null) && !q.IsFolder())
                            .Select(q => new QueryDefinition(q))
                        ?? Enumerable.Empty<IQueryDefinition>();
                }))
        {
        }
    }
}