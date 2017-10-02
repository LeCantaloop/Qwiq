using System;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryFolder : Qwiq.QueryFolder
    {
        internal QueryFolder(QueryHierarchyItem queryFolder, Func<string, QueryHierarchyItem> folderExpansionFunc)
            : base(
                queryFolder.Id,
                queryFolder.Name,
                new QueryFolderCollection(() =>
                {
                    if (queryFolder.Children == null)
                    {
                        return Enumerable.Empty<IQueryFolder>();
                    }
                    
                    return queryFolder
                        .Children
                        .Where(q => q?.IsFolder != null && q.IsFolder.Value)
                        .Select(q => q.HasChildren.HasValue && q.HasChildren.Value && q.Children == null ? folderExpansionFunc(q.Path) : q)
                        .Select(q => new QueryFolder(q, folderExpansionFunc));
                }),
                new QueryDefinitionCollection(() =>
                {
                    return queryFolder.Children?.Where(q => !q.IsFolder.HasValue || !q.IsFolder.Value).Select(q => new QueryDefinition(q)) ?? Enumerable.Empty<IQueryDefinition>();
                }))
        {
        }
    }
}