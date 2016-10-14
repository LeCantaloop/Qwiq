using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mapper
{
    public abstract class WorkItemMapperStrategyBase : IWorkItemMapperStrategy
    {
        public virtual void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
            {
                Map(targetWorkItemType, workItemMapping.Key, workItemMapping.Value, workItemMapper);
            }
        }

        public virtual void Map<T>(
            IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings,
            IWorkItemMapper workItemMapper) where T : IIdentifiable, new()
        {
            Map(typeof(T), workItemMappings.Select(s => new KeyValuePair<IWorkItem, IIdentifiable>(s.Key, s.Value)), workItemMapper);
        }

        protected virtual void Map(
            Type targetWorkItemType,
            IWorkItem sourceWorkItem,
            IIdentifiable targetWorkItem,
            IWorkItemMapper workItemMapper)
        {
            Map(
                targetWorkItemType,
                new[] { new KeyValuePair<IWorkItem, IIdentifiable>(sourceWorkItem, targetWorkItem) },
                workItemMapper);
        }

        protected virtual void Map<T>(IWorkItem sourceWorkItem, T targetWorkItem, IWorkItemMapper workItemMapper)
        {
            Map(typeof(T), sourceWorkItem, (IIdentifiable)targetWorkItem, workItemMapper);
        }
    }
}

