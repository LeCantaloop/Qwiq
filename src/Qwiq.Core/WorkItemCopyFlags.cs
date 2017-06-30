using System;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Flags specifying optional work item data that should be copied.
    /// </summary>
    /// <seealso cref="TeamFoundation.WorkItemTracking.Client.WorkItemCopyFlags"/>
    [Flags]
    public enum WorkItemCopyFlags
    {
        None = 0,
        CopyFiles = 1,
        CopyLinks = 2,
    }
}
