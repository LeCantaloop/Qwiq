using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IWorkItemTypeCollection : IEnumerable<IWorkItemType>
    {
        IWorkItemType this[string typeName] { get; }
    }
}