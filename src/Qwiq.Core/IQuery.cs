using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IQuery
    {
        IEnumerable<IWorkItem> RunQuery();
        IEnumerable<IWorkItemLinkInfo> RunLinkQuery();

        IEnumerable<IWorkItemLinkTypeEnd> GetLinkTypes();
    }
}
