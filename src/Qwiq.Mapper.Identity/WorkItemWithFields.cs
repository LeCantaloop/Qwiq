using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Qwiq.Mapper
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct WorkItemWithFields
    {
        public IWorkItem WorkItem { get; set; }
        public List<WorkItemField> ValidFields { get; set; }
    }
}
