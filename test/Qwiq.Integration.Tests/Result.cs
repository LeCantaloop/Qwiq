using System;
using System.Collections.Generic;

namespace Qwiq
{
    public class Result : IDisposable
    {
        private IWorkItem? _workItem = null!;

        public IEnumerable<IWorkItemLinkInfo>? Links { get; set; } = null!;

        public IWorkItem? WorkItem
        {
            get => _workItem;
            set
            {
                _workItem = value;
                WorkItems = value != null ? new WorkItemCollection(new List<IWorkItem>(new[] { value })) : null;
            }
        }

        public IWorkItemCollection? WorkItems { get; set; } = null!;

        public IWorkItemStore? WorkItemStore { get; set; } = null!;

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