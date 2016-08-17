using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> workItemMappings, IWorkItemMapper workItemMapper);
        void Map<T>(IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings, IWorkItemMapper workItemMapper) where T : IIdentifiable, new();
    }
}