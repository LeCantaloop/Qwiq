using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryFolder : Qwiq.QueryFolder
    {
        internal QueryFolder([NotNull] QueryHierarchyItem queryFolder, [NotNull] Func<QueryHierarchyItem, QueryHierarchyItem> folderExpansionFunc)
            : base(
                queryFolder.Id,
                queryFolder.Name,
                new QueryFolderCollection(() =>
                {
                    return
                        queryFolder
                            .Children?
                            .Where(q => q != null && q.IsFolder())
                            .Select(q => !q.IsExpanded() ? folderExpansionFunc(q) : q)
                            .Select(q => new QueryFolder(q, folderExpansionFunc))
                        ?? Enumerable.Empty<IQueryFolder>();
                }),
                new QueryDefinitionCollection(() =>
                {
                    return
                        queryFolder
                            .Children?
                            .Where(q => q != null && !q.IsFolder())
                            .Select(q => new QueryDefinition(q))
                        ?? Enumerable.Empty<IQueryDefinition>();
                }))
        {
        }
    }
}