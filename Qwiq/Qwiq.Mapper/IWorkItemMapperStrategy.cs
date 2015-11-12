using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targeWorkItemType, IEnumerable<WorkItemMapping> workItemMappings, IWorkItemMapper workItemMapper);
    }
}