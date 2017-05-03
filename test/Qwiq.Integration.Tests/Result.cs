using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class Result : IDisposable
    {
        private IWorkItem _workItem;

        public IEnumerable<IWorkItemLinkInfo> Links { get; set; }

        public IWorkItem WorkItem
        {
            get => _workItem;
            set
            {
                _workItem = value;
                WorkItems = new WorkItemCollection(new List<IWorkItem>(new[] { value }));
            }
        }

        public IWorkItemCollection WorkItems { get; set; }

        public IWorkItemStore WorkItemStore { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) WorkItemStore?.Dispose();

            WorkItemStore = null;
            _workItem = null;
            WorkItems = null;
            Links = null;
        }
    }
}