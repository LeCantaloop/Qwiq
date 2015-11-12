using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, object>> workItemMappings, IWorkItemMapper workItemMapper);
    }
}