using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    public interface IQuery
    {
        IEnumerable<IWorkItem> RunQuery();
        IEnumerable<IWorkItemLinkInfo> RunLinkQuery();
    }
}