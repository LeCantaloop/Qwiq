using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Identity.Mapper
{
    internal class WorkItemKeyComparer : IEqualityComparer<IWorkItem>
    {
        public bool Equals(IWorkItem x, IWorkItem y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IWorkItem obj)
        {
            return obj.Id;
        }
    }
}