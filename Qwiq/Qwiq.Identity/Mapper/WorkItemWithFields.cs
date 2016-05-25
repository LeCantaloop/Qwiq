using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Identity.Mapper
{
    internal struct WorkItemWithFields
    {
        public IWorkItem WorkItem { get; set; }
        public IEnumerable<WorkItemField> ValidFields { get; set; }
    }
}