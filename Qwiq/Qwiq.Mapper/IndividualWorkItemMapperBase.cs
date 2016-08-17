using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mapper
{
    public abstract class IndividualWorkItemMapperBase : IWorkItemMapperStrategy
    {
        public virtual void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, object>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
            {
                Map(targetWorkItemType, workItemMapping.Key, workItemMapping.Value, workItemMapper);
            }
        }

        public virtual void Map<T>(
            Type targetWorkItemType,
            IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings,
            IWorkItemMapper workItemMapper) where T : new()
        {
            foreach (var workItemMapping in workItemMappings)
            {
                Map(targetWorkItemType, workItemMapping.Key, workItemMapping.Value, workItemMapper);
            }
        }

        protected abstract void Map(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper);

        protected virtual void Map<T>(Type targetWorkItemType, IWorkItem sourceWorkItem, T targetWorkItem, IWorkItemMapper workItemMapper)
        {
            Map(targetWorkItemType, sourceWorkItem, (object)targetWorkItem, workItemMapper);
        }
    }
}
