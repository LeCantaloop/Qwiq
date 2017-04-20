using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> workItemMappings, IWorkItemMapper workItemMapper);
        void Map<T>(IEnumerable<KeyValuePair<IWorkItem, T>> workItemMappings, IWorkItemMapper workItemMapper) where T : IIdentifiable<int?>, new();
    }
}
