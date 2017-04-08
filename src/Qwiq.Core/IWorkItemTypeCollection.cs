using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IEnumerable<IWorkItemType>
    {
        int Count { get; }

        IWorkItemType this[string typeName] { get; }
    }
}