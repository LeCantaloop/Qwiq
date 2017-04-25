using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Qwiq.Mapper
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct WorkItemWithFields
    {
        public IWorkItem WorkItem { get; set; }
        public IEnumerable<WorkItemField> ValidFields { get; set; }
    }
}
