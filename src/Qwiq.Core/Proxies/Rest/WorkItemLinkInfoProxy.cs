using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly WorkItemLink _item;

        internal WorkItemLinkInfoProxy(WorkItemLink item)
        {
            _item = item;            
        }

        public bool IsLocked => throw new NotImplementedException();

        public int LinkTypeId => throw new NotImplementedException();

        public int SourceId => (_item.Source?.Id).GetValueOrDefault();

        public int TargetId => (_item.Target?.Id).GetValueOrDefault();
    }
}
