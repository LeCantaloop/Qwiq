using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mapper
{
    public abstract class IndividualWorkItemMapperBase : IWorkItemMapperStrategy
    {
        public void Map(Type targeWorkItemType, IEnumerable<WorkItemMapping> workItemMappings, IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
            {
                Map(targeWorkItemType, workItemMapping.WorkItem, workItemMapping.MappedWorkItem, workItemMapper);
            }
        }

        protected abstract void Map(Type targeWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper);
    }
}
