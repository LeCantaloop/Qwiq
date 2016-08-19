using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Mapper;

namespace Microsoft.IE.Qwiq.Relatives.Mapper
{
    public class ParentIdMapperStrategy : WorkItemMapperStrategyBase
    {
        private const int SelfReferenceLinkId = 0;
        private const string ParentLinkQueryFormat = @"
SELECT
    *
FROM
    WorkItemLinks
WHERE
    (Source.[System.Id] In ({0}))
    AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Reverse')
ASOF
    '{1}'";

        private readonly IWorkItemStore _workItemStore;

        public ParentIdMapperStrategy(IWorkItemStore workItemStore)
        {
            _workItemStore = workItemStore;
        }

        public override void Map(Type targeWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var workingSet = workItemMappings.ToList();
            var parentIdField = targeWorkItemType.GetProperty("ParentId");

            if (!workingSet.Any() || parentIdField == null)
            {
                return;
            }

            var workItems = workingSet.Select(wi => wi.Key).ToList();
            var ids = workItems.Select(wi => wi.Id);
            var asOf = workItems.Max(wi => wi.ChangedDate);
            var childToParentLinks =
                _workItemStore.QueryLinks(string.Format(ParentLinkQueryFormat, string.Join(",", ids), asOf))
                    .Where(link => link.LinkTypeId != SelfReferenceLinkId);
            var childToParentDictionary = childToParentLinks.ToDictionary(link => link.SourceId, link => link.TargetId);

            foreach (var workItemMapping in workingSet)
            {
                var childId = workItemMapping.Key.Id;
                if (childToParentDictionary.ContainsKey(childId))
                {
                    parentIdField.SetValue(workItemMapping.Value, childToParentDictionary[childId]);
                }
            }
        }
    }
}
