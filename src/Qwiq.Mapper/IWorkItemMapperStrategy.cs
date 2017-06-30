using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targetWorkItemType, IDictionary<IWorkItem, IIdentifiable<int?>> workItemMappings, IWorkItemMapper workItemMapper);
        void Map<T>(IDictionary<IWorkItem, T> workItemMappings, IWorkItemMapper workItemMapper) where T : IIdentifiable<int?>, new();
    }
}
