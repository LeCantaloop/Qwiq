using System;

namespace Microsoft.Qwiq
{
    [Flags]
    public enum WorkItemCopyFlags
    {
        None = 0,
        CopyFiles = 1,
        CopyLinks = 2,
    }
}
