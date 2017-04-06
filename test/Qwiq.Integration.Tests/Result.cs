using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Integration.Tests
{
    public class Result : IDisposable
    {
        public IWorkItem WorkItem { get; set; }

        public IEnumerable<IWorkItemLinkInfo> Links { get; set; }

        public IWorkItemStore WorkItemStore { get; set; }

        public void Dispose()
        {
            WorkItemStore?.Dispose();
        }
    }
}
