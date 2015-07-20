using System;

namespace Microsoft.IE.Qwiq.Mapper
{
    public interface IWorkItemMapperStrategy
    {
        void Map(Type targeWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper);
    }
}