using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        internal WorkItemTypeProxy(string name)
        {
            Name = name;
        }

        public string Description { get; }

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            throw new NotImplementedException();
        }
    }
}
