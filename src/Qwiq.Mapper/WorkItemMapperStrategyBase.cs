using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mapper
{
    public abstract class WorkItemMapperStrategyBase : IWorkItemMapperStrategy
    {
        public virtual void Map(
            Type targetWorkItemType,
            IDictionary<IWorkItem, IIdentifiable<int?>> workItemMappings,
            IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
                Map(targetWorkItemType, workItemMapping.Key, workItemMapping.Value, workItemMapper);
        }

        public virtual void Map<T>(IDictionary<IWorkItem, T> workItemMappings, IWorkItemMapper workItemMapper)
            where T : IIdentifiable<int?>, new()
        {
            Map(typeof(T), workItemMappings.ToDictionary(k => k.Key, e => (IIdentifiable<int?>)e.Value, Comparer.WorkItem), workItemMapper);
        }

        protected virtual void Map(
            Type targetWorkItemType,
            IWorkItem sourceWorkItem,
            IIdentifiable<int?> targetWorkItem,
            IWorkItemMapper workItemMapper)
        {
            Map(targetWorkItemType, new Dictionary<IWorkItem, IIdentifiable<int?>>(Comparer.WorkItem) { { sourceWorkItem, targetWorkItem } }, workItemMapper);
        }

        protected virtual void Map<T>(IWorkItem sourceWorkItem, T targetWorkItem, IWorkItemMapper workItemMapper)
            where T : IIdentifiable<int?>, new()
        {
            Map(typeof(T), sourceWorkItem, targetWorkItem, workItemMapper);
        }
    }
}