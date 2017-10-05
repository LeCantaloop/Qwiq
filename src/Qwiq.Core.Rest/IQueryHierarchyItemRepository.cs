using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal interface IQueryHierarchyItemRepository
    {
        IEnumerable<QueryHierarchyItem> Get();
        QueryHierarchyItem EnsureExpanded(QueryHierarchyItem item);
    }
}