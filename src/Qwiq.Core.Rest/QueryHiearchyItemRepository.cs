using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Rest
{
    internal class QueryHiearchyItemRepository : IQueryHierarchyItemRepository
    {
        //BUGBUG: There is a bug in the GetQueryAsync in vsts where if a folder contains a '+' character it will return 404, even if the folder exists see here: https://developercommunity.visualstudio.com/content/problem/123660/when-a-saved-query-folder-contains-a-character-it.html
        private const string BadPathCharacter = "+";

        //The VSTS Rest api for shared queries only allows for expanding folder structure 2 deep, https://www.visualstudio.com/en-us/docs/integrate/api/wit/queries
        private const int MaxQueryFolderExpansionDepth = 2;

        private readonly Lazy<WorkItemTrackingHttpClient> _workItemStore;
        private readonly Guid _projectId;

        public QueryHiearchyItemRepository([NotNull] Lazy<WorkItemTrackingHttpClient> workItemStore, Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(projectId));
            }

            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));
            _projectId = projectId;
        }

        public IEnumerable<QueryHierarchyItem> Get()
        {
            return
                _workItemStore
                    .Value
                    .GetQueriesAsync(_projectId, QueryExpand.Wiql, MaxQueryFolderExpansionDepth)
                    .GetAwaiter()
                    .GetResult();
        }

        public QueryHierarchyItem EnsureExpanded(QueryHierarchyItem item)
        {
            if (item.IsExpanded())
            {
                return item;
            }

            try
            {
                return
                    _workItemStore
                        .Value
                        .GetQueryAsync(_projectId, item.Path, QueryExpand.Wiql, MaxQueryFolderExpansionDepth)
                        .GetAwaiter()
                        .GetResult();
            }
            catch (VssServiceResponseException ex)
            {
                if (ex.HttpStatusCode == HttpStatusCode.NotFound && item.Path.Contains(BadPathCharacter))
                {
                    throw new InvalidOperationException($"Unable to expand the saved query folder {item.Path} due to a bug in the Vso Query Service. Unable to retrieve folders with '{BadPathCharacter}' in the name. See bug, https://developercommunity.visualstudio.com/content/problem/123660/when-a-saved-query-folder-contains-a-character-it.html.", ex);
                }

                throw;
            }
        }
    }
}
