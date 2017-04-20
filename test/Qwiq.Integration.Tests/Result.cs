using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Integration.Tests
{
    public class Result : IDisposable
    {
        private IWorkItem _workItem;

        public IWorkItem WorkItem
        {
            get => _workItem;
            set
            {
                _workItem = value;
                WorkItems = new WorkItemCollection(new[]{value});
            }
        }

        public IWorkItemCollection WorkItems { get; set; }

        public IEnumerable<IWorkItemLinkInfo> Links { get; set; }

        public IWorkItemStore WorkItemStore { get; set; }

        public void Dispose()
        {
            WorkItemStore?.Dispose();
        }
    }
}
